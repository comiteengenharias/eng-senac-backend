using EngenhariasSenac.Banco;
using EngenhariasSenac.Database;
using EngenhariasSenac.Dtos;
using EngenhariasSenac.Models;
using System.Text.Json;

namespace EngenhariasSenac.Helpers;

/// <summary>
/// Classe responsável por calcular ratings e rankings de projetos
/// </summary>
public static class ProjectRankingCalculator
{
    /// <summary>
    /// Pesos para cálculo da média ponderada
    /// </summary>
    private const decimal BANCA_WEIGHT = 0.7m;
    private const decimal FEIRA_WEIGHT = 0.25m;
    private const decimal STUDENT_WEIGHT = 0.05m;

    /// <summary>
    /// Quantidades mínimas de avaliações por categoria
    /// </summary>
    private const int MIN_BANCA_REVIEWS = 2;
    private const int MIN_FEIRA_REVIEWS = 3;
    private const int MIN_STUDENT_REVIEWS = 5;

    /// <summary>
    /// Calcula os ratings e ranking de um projeto específico
    /// </summary>
    public static ProjectFinalRatingDto CalculateProjectRating(
        int projectTeamId,
        int semester,
        EngenhariasSenacContext context)
    {
        try
        {
            // Obter todas as avaliações de banca
            var dalProjectNotes = new DAL<ProjectNote>(context);
            var bancaNotes = dalProjectNotes
                .SelectWhereList(a => a.EvaluatedProject == projectTeamId && a.EvaluateType == EvaluateType.Banca)
                .ToList();

            // Obter todas as avaliações de feira
            var feiraNotes = dalProjectNotes
                .SelectWhereList(a => a.EvaluatedProject == projectTeamId && a.EvaluateType == EvaluateType.Feira)
                .ToList();

            // Obter todas as avaliações de alunos
            var dalProjectAssessments = new DAL<ProjectAssessment>(context);
            var studentAssessments = dalProjectAssessments
                .SelectWhereList(a => a.EvaluatedProject == projectTeamId)
                .ToList();

            // Calcular médias e contar avaliações
            var bancaRating = CalculateRating(bancaNotes, MIN_BANCA_REVIEWS, note => ParseAssessmentPublic(note.Assessment));
            var feiraRating = CalculateRating(feiraNotes, MIN_FEIRA_REVIEWS, note => ParseAssessmentPublic(note.Assessment));
            var studentRating = CalculateRating(studentAssessments, MIN_STUDENT_REVIEWS, assessment => (decimal)assessment.Assessment);

            // Calcular média ponderada
            var finalAverage = CalculateWeightedAverage(bancaRating.Average, feiraRating.Average, studentRating.Average);

            // Calcular ranking
            var ranking = CalculateRanking(projectTeamId, semester, finalAverage, bancaRating, feiraRating, studentRating, context);

            return new ProjectFinalRatingDto
            {
                BancaRating = bancaRating,
                FeeraRating = feiraRating,
                StudentRating = studentRating,
                FinalAverage = finalAverage,
                RankingPosition = ranking.Position,
                RankingAverage = ranking.Average
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao calcular rating do projeto: " + ex.Message, ex);
        }
    }

    /// <summary>
    /// Calcula os ratings de um projeto sem incluir ranking (para evitar recursão)
    /// </summary>
    private static (ProjectRatingDto Banca, ProjectRatingDto Feira, ProjectRatingDto Student) CalculateProjectRatingsOnly(
        int projectTeamId,
        EngenhariasSenacContext context)
    {
        var dalProjectNotes = new DAL<ProjectNote>(context);
        var bancaNotes = dalProjectNotes
            .SelectWhereList(a => a.EvaluatedProject == projectTeamId && a.EvaluateType == EvaluateType.Banca)
            .ToList();

        var feiraNotes = dalProjectNotes
            .SelectWhereList(a => a.EvaluatedProject == projectTeamId && a.EvaluateType == EvaluateType.Feira)
            .ToList();

        var dalProjectAssessments = new DAL<ProjectAssessment>(context);
        var studentAssessments = dalProjectAssessments
            .SelectWhereList(a => a.EvaluatedProject == projectTeamId)
            .ToList();

        var bancaRating = CalculateRating(bancaNotes, MIN_BANCA_REVIEWS, note => ParseAssessmentPublic(note.Assessment));
        var feiraRating = CalculateRating(feiraNotes, MIN_FEIRA_REVIEWS, note => ParseAssessmentPublic(note.Assessment));
        var studentRating = CalculateRating(studentAssessments, MIN_STUDENT_REVIEWS, assessment => (decimal)assessment.Assessment);

        return (bancaRating, feiraRating, studentRating);
    }

    /// <summary>
    /// Calcula o rating para uma categoria específica
    /// Se não atingir o mínimo, divide pelo mínimo. Caso contrário, divide pela quantidade real.
    /// </summary>
    private static ProjectRatingDto CalculateRating<T>(
        List<T> items,
        int minimumRequired,
        Func<T, decimal> getAssessmentValue)
    {
        var count = items.Count;
        decimal average = 0m;

        if (count > 0)
        {
            var sum = items.Sum(getAssessmentValue);
            
            // Se não atingiu o mínimo, divide pelo mínimo
            // Caso contrário, divide pela quantidade real
            var divisor = count < minimumRequired ? minimumRequired : count;
            average = sum / divisor;
        }

        // Arredondar para 2 casas decimais
        average = Math.Round(average, 2);

        return new ProjectRatingDto
        {
            AssessmentCount = $"{count}/{minimumRequired}",
            Average = average
        };
    }

    /// <summary>
    /// Converte o Assessment (string JSON) para decimal calculando a média das categorias
    /// </summary>
    public static decimal ParseAssessmentPublic(string assessment)
    {
        try
        {
            // Tentar fazer parse como JSON
            using (JsonDocument doc = JsonDocument.Parse(assessment))
            {
                var root = doc.RootElement;
                
                if (root.ValueKind == JsonValueKind.Object)
                {
                    var values = new List<decimal>();
                    
                    foreach (var property in root.EnumerateObject())
                    {
                        if (property.Value.ValueKind == JsonValueKind.Number)
                        {
                            if (property.Value.TryGetDecimal(out var value))
                            {
                                values.Add(value);
                            }
                        }
                    }
                    
                    // Se conseguiu parsear valores numéricos, retorna a média
                    if (values.Count > 0)
                    {
                        return Math.Round(values.Average(), 2);
                    }
                }
            }
        }
        catch
        {
            // Se não conseguir fazer parse como JSON, tenta como número direto
        }

        // Fallback: tentar fazer parse como número simples
        if (decimal.TryParse(assessment, out var result))
            return result;
        
        return 0m;
    }

    /// <summary>
    /// Calcula a média ponderada das três categorias
    /// </summary>
    private static decimal CalculateWeightedAverage(
        decimal bancaAverage,
        decimal feiraAverage,
        decimal studentAverage)
    {
        var weighted = (bancaAverage * BANCA_WEIGHT) +
                       (feiraAverage * FEIRA_WEIGHT) +
                       (studentAverage * STUDENT_WEIGHT);

        return Math.Round(weighted, 2);
    }

    /// <summary>
    /// Calcula a posição no ranking do grupo no semestre
    /// </summary>
    private static (int Position, decimal Average) CalculateRanking(
        int projectTeamId,
        int semester,
        decimal finalAverage,
        ProjectRatingDto bancaRating,
        ProjectRatingDto feiraRating,
        ProjectRatingDto studentRating,
        EngenhariasSenacContext context)
    {
        try
        {
            // Obter todos os dados necessários em uma única query
            var projectIdsInSemester = context.ProjectTeams
                .Where(pt => pt.Semester == semester)
                .Select(pt => pt.CodTeam)
                .ToList();

            var allProjectNotes = context.ProjectNotes
                .Where(p => projectIdsInSemester.Contains(p.EvaluatedProject))
                .ToList();

            var allStudentAssessments = context.ProjectAssessment
                .Where(p => projectIdsInSemester.Contains(p.EvaluatedProject))
                .ToList();

            var allProjectsInSemester = context.ProjectTeams
                .Where(p => p.Semester == semester)
                .ToList();

            if (allProjectsInSemester.Count == 0)
                return (1, finalAverage);

            // Calcular ratings para todos os projetos em memória (sem queries adicionais)
            var projectRatings = new List<ProjectTeamRanking>();

            foreach (var project in allProjectsInSemester)
            {
                var projectBancaNotes = allProjectNotes
                    .Where(n => n.EvaluatedProject == project.CodTeam && n.EvaluateType == EvaluateType.Banca)
                    .ToList();

                var projectFeiraNotes = allProjectNotes
                    .Where(n => n.EvaluatedProject == project.CodTeam && n.EvaluateType == EvaluateType.Feira)
                    .ToList();

                var projectStudentAssessments = allStudentAssessments
                    .Where(a => a.EvaluatedProject == project.CodTeam)
                    .ToList();

                // Calcular médias em memória
                decimal bancaSum = projectBancaNotes.Count > 0 ? projectBancaNotes.Sum(n => ParseAssessmentPublic(n.Assessment)) : 0m;
                int bancaDivisor = projectBancaNotes.Count < MIN_BANCA_REVIEWS ? MIN_BANCA_REVIEWS : projectBancaNotes.Count;
                var bancaAvg = projectBancaNotes.Count > 0 ? Math.Round(bancaSum / bancaDivisor, 2) : 0m;

                decimal feiraSum = projectFeiraNotes.Count > 0 ? projectFeiraNotes.Sum(n => ParseAssessmentPublic(n.Assessment)) : 0m;
                int feiraDivisor = projectFeiraNotes.Count < MIN_FEIRA_REVIEWS ? MIN_FEIRA_REVIEWS : projectFeiraNotes.Count;
                var feiraAvg = projectFeiraNotes.Count > 0 ? Math.Round(feiraSum / feiraDivisor, 2) : 0m;

                decimal studentSum = projectStudentAssessments.Count > 0 ? projectStudentAssessments.Sum(a => (decimal)a.Assessment) : 0m;
                int studentDivisor = projectStudentAssessments.Count < MIN_STUDENT_REVIEWS ? MIN_STUDENT_REVIEWS : projectStudentAssessments.Count;
                var studentAvg = projectStudentAssessments.Count > 0 ? Math.Round(studentSum / studentDivisor, 2) : 0m;

                var projectFinalAverage = CalculateWeightedAverage(bancaAvg, feiraAvg, studentAvg);
                // Armazenar também o valor sem arredondamento para desempate correto
                var projectFinalAverageUnrounded = (bancaAvg * BANCA_WEIGHT) + (feiraAvg * FEIRA_WEIGHT) + (studentAvg * STUDENT_WEIGHT);

                projectRatings.Add(new ProjectTeamRanking
                {
                    ProjectTeamId = project.CodTeam,
                    FinalAverage = projectFinalAverage,
                    FinalAverageUnrounded = projectFinalAverageUnrounded,
                    BancaCount = projectBancaNotes.Count,
                    FeiraCount = projectFeiraNotes.Count,
                    StudentCount = projectStudentAssessments.Count
                });
            }

            // Ordenar por critério de desempate
            var rankedProjects = RankProjects(projectRatings);

            // Encontrar a posição do projeto atual
            var position = rankedProjects.FindIndex(p => p.ProjectTeamId == projectTeamId) + 1;

            return (position, finalAverage);
        }
        catch
        {
            // Em caso de erro, retornar posição 1
            return (1, finalAverage);
        }
    }

    /// <summary>
    /// Ordena os projetos pelo critério de ranking
    /// Ordem de desempate: 
    /// 1. Média final arredondada (descendente)
    /// 2. Se empatar na média: quantidade de avaliações Feira (descendente)
    /// 3. Se empatar na feira: quantidade de avaliações Aluno (descendente)
    /// 4. Se empatar em tudo: sorteio aleatório
    /// </summary>
    private static List<ProjectTeamRanking> RankProjects(List<ProjectTeamRanking> projects)
    {
        var random = new Random();

        // Agrupar por média arredondada primeiro, depois aplicar desempate dentro de cada grupo
        return projects
            .GroupBy(p => p.FinalAverage)                                    // Agrupa por média arredondada
            .OrderByDescending(g => g.Key)                                  // Ordena grupos por média (descendente)
            .SelectMany(g => g
                .OrderByDescending(p => p.FeiraCount)                       // Dentro do grupo: por Feira
                .ThenByDescending(p => p.StudentCount)                     // Depois por Alunos
                .ThenBy(p => random.Next())                                // Depois sorteio
            )
            .ToList();
    }

    /// <summary>
    /// Classe auxiliar para armazenar dados de ranking
    /// </summary>
    private class ProjectTeamRanking
    {
        public int ProjectTeamId { get; set; }
        public decimal FinalAverage { get; set; }
        public decimal FinalAverageUnrounded { get; set; }
        public int BancaCount { get; set; }
        public int FeiraCount { get; set; }
        public int StudentCount { get; set; }
    }
}

