using EngenhariasSenac.Banco;
using EngenhariasSenac.Database;
using EngenhariasSenac.Helpers;
using EngenhariasSenac.Models;
using EngenhariasSenac.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
namespace EngenhariasSenac.Controllers;

public class TeacherController : ControllerBase
{
    public static Teacher? GetAuthenticatedTeacher(HttpContext http)
    {
        try
        {
            var codClaim = http.User.FindFirst("CodTeacher")?.Value;
            if (string.IsNullOrEmpty(codClaim))
                return null;

            int codTeacher = int.Parse(codClaim);

            var context = new EngenhariasSenacContext();
            var dal = new DAL<Teacher>(context);
            var teacher = dal.SelectWhere(s => s.CodTeacher == codTeacher);

            return teacher;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Retorna os dados do professor autenticado.
    /// </summary>
    /// <param name="http">HttpContext com token de autenticação.</param>
    /// <returns>Objeto com informações do professor.</returns>
    [Authorize(Roles = "Teacher")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public static IResult GetMyTeacherData(HttpContext http)
    {
        try
        {
            var teacher = GetAuthenticatedTeacher(http);
            if (teacher == null)
                return Results.NotFound("Professor não encontrado");

            return Results.Ok(new
            {
                teacher = teacher
            });
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao buscar dados do professor: " + ex.Message);
        }
    }

    /// <summary>
    /// Altera a senha do professor autenticado.
    /// </summary>
    /// <param name="data">Objeto com `CurrentPsw` e `NewPsw`.</param>
    /// <param name="http">HttpContext com token de autenticação.</param>
    /// <returns>Mensagem de sucesso ou erro.</returns>
    [Authorize(Roles = "Teacher")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public static IResult PostChangePassword([FromBody] ChangePasswordDto data, HttpContext http)
    {
        try
        {
            var teacher = GetAuthenticatedTeacher(http);
            if (teacher == null)
                return Results.NotFound("Professor não encontrado");

            var context = new EngenhariasSenacContext();
            var dalTeacher = new DAL<Teacher>(context);

            // verifica a senha atual
            if (!PasswordHasher.VerifyPassword(data.CurrentPsw, teacher.Password))
                return Results.Conflict("Senha atual incorreta");

            // criptografa nova senha
            var hashedPsw = PasswordHasher.HashPassword(data.NewPsw);
            teacher.Password = hashedPsw;

            dalTeacher.Update(teacher);

            return Results.Ok("Senha alterada com sucesso");
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao alterar senha: " + ex.Message);
        }
    }

    /// <summary>
    /// Retorna a lista de projetos disponíveis para avaliação pelo professor.
    /// </summary>
    /// <param name="filterDto">Filtro opcional por semestre e nome do grupo.</param>
    /// <param name="http">HttpContext com token de autenticação.</param>
    /// <returns>Lista de projetos com informações resumidas.</returns>
    [Authorize(Roles = "Teacher")]
    [HttpPost("/api/teacher/projects")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public static IResult GetProjectsData([FromBody] ProjectsFilterDto? filterDto, HttpContext http)
    {
        try
        {
            var teacher = GetAuthenticatedTeacher(http);
            if (teacher == null)
                return Results.NotFound("Professor não encontrado");

            filterDto ??= new ProjectsFilterDto();

            var context = new EngenhariasSenacContext();
            var dalProject = new DAL<ProjectTeam>(context);
            var projects = dalProject.SelectWhereList(
                a => (!filterDto.Semester.HasValue || a.Semester == filterDto.Semester.Value)
                    && (string.IsNullOrEmpty(filterDto.GroupName) || a.GroupName.ToLower().Contains(filterDto.GroupName.ToLower())),
                q => q.OrderBy(p => p.Semester).ThenBy(p => p.GroupName)
            );

            var projectIds = projects.Select(p => p.CodTeam).ToList();

            var bancaEvaluatedProjects = context.ProjectNotes
                .Where(n => n.TeacherEvaluator == teacher.CodTeacher
                    && projectIds.Contains(n.EvaluatedProject)
                    && n.EvaluateType == EvaluateType.Banca)
                .Select(n => n.EvaluatedProject)
                .ToHashSet();

            var feiraEvaluatedProjects = context.ProjectNotes
                .Where(n => n.TeacherEvaluator == teacher.CodTeacher
                    && projectIds.Contains(n.EvaluatedProject)
                    && n.EvaluateType == EvaluateType.Feira)
                .Select(n => n.EvaluatedProject)
                .ToHashSet();

            var allProjects = new List<object>();

            foreach (var item in projects)
            {
                // Verificar em memória com O(1) lookup usando HashSet!
                var alreadyBankingEvaluated = bancaEvaluatedProjects.Contains(item.CodTeam);
                var alreadyFairEvaluated = feiraEvaluatedProjects.Contains(item.CodTeam);

                allProjects.Add(new
                {
                    Project = item,
                    AlreadyBankingEvaluated = alreadyBankingEvaluated,
                    AlreadyFairEvaluated = alreadyFairEvaluated
                });
            }

            return Results.Ok(allProjects);
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao buscar dados: " + ex.Message);
        }
    }

    /// <summary>
    /// Envia avaliação de projeto feita pelo professor (nota final e comentário).
    /// </summary>
    /// <param name="data">Objeto com `ProjectId`, `Assessment`, `Comment` e `EvaluateType`.</param>
    /// <param name="http">HttpContext com token de autenticação.</param>
    /// <returns>Mensagem de sucesso ou erro.</returns>
    [Authorize(Roles = "Teacher")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public static IResult PostEvaluateProjects([FromBody] ProjectsNotesDto data, HttpContext http)
    {
        try
        {
            var teacher = GetAuthenticatedTeacher(http);
            if (teacher == null)
                return Results.NotFound("Professor não encontrado");

            var context = new EngenhariasSenacContext();

            var dalProject = new DAL<ProjectTeam>(context);
            var project = dalProject.SelectWhere(a => a.CodTeam == data.ProjectId);

            if (data.ProjectId <= 0 || project == null)
                return Results.BadRequest("Projeto não encontrado");

            // Obtém a hora atual de Brasília
            TimeZoneInfo brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brasiliaTimeZone);

            var dalProjNotes = new DAL<ProjectNote>(context);

            var newNote = (new ProjectNote
            {
                TeacherEvaluator = teacher.CodTeacher,
                EvaluatedProject = data.ProjectId,
                Assessment = data.Assessment,
                Comment = data.Comment,
                Datetime = now,
                EvaluateType = data.EvaluateType,
            });

            dalProjNotes.Insert(newNote);

            return Results.Ok("Avaliação registrada com sucesso");
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao registrar avaliação: " + ex.Message);
        }
    }

    /// <summary>
    /// Retorna os melhores projetos do semestre informado.
    /// </summary>
    /// <param name="filterDto">Filtro opcional por semestre e nome do grupo.</param>
    /// <param name="http">HttpContext com token de autenticação.</param>
    /// <returns>Lista ordenada dos melhores projetos.</returns>
    [Authorize(Roles = "Teacher")]
    [HttpPost("/api/teacher/top-projects-ranking")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public static IResult GetTopProjectsBySemester([FromBody] ProjectsFilterDto? filterDto, HttpContext http)
    {
        try
        {
            var teacher = GetAuthenticatedTeacher(http);
            if (teacher == null)
                return Results.NotFound("Professor não encontrado");

            filterDto ??= new ProjectsFilterDto();

            var context = new EngenhariasSenacContext();

            // Obter todos os dados em 3 queries otimizadas
            var projectsInSemester = context.ProjectTeams
                .Where(p => !filterDto.Semester.HasValue || p.Semester == filterDto.Semester.Value)
                .Where(p => string.IsNullOrEmpty(filterDto.GroupName) || p.GroupName.ToLower().Contains(filterDto.GroupName.ToLower()))
                .ToList();

            if (projectsInSemester.Count == 0)
                return Results.Ok(new List<object>());

            var projectIds = projectsInSemester.Select(p => p.CodTeam).ToList();

            var allProjectNotes = context.ProjectNotes
                .Where(n => projectIds.Contains(n.EvaluatedProject))
                .ToList();

            var allStudentAssessments = context.ProjectAssessment
                .Where(a => projectIds.Contains(a.EvaluatedProject))
                .ToList();

            // Calcular ratings em memória (sem queries adicionais)
            var projectRatingsIntermediate = new List<ProjectRatingIntermediate>();

            foreach (var project in projectsInSemester)
            {
                try
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

                    // Calcular médias
                    int bancaCount = projectBancaNotes.Count;
                    decimal bancaSum = bancaCount > 0
                        ? projectBancaNotes.Sum(n => ProjectRankingCalculator.ParseAssessmentPublic(n.Assessment))
                        : 0m;
                    int bancaDivisor = bancaCount < 2 ? 2 : bancaCount;
                    decimal bancaAvg = bancaCount > 0
                        ? Math.Round(bancaSum / bancaDivisor, 2)
                        : 0m;

                    int feiraCount = projectFeiraNotes.Count;
                    decimal feiraSum = feiraCount > 0
                        ? projectFeiraNotes.Sum(n => ProjectRankingCalculator.ParseAssessmentPublic(n.Assessment))
                        : 0m;
                    int feiraDivisor = feiraCount < 3 ? 3 : feiraCount;
                    decimal feiraAvg = feiraCount > 0
                        ? Math.Round(feiraSum / feiraDivisor, 2)
                        : 0m;

                    int studentCount = projectStudentAssessments.Count;
                    decimal studentSum = studentCount > 0
                        ? projectStudentAssessments.Sum(a => (decimal)a.Assessment)
                        : 0m;
                    int studentDivisor = studentCount < 3 ? 3 : studentCount;
                    decimal studentAvg = studentCount > 0
                        ? Math.Round(studentSum / studentDivisor, 2)
                        : 0m;

                    // Média ponderada
                    var finalAverage = Math.Round((bancaAvg * 0.7m) + (feiraAvg * 0.25m) + (studentAvg * 0.05m), 2);

                    projectRatingsIntermediate.Add(new ProjectRatingIntermediate
                    {
                        ProjectName = project.GroupName,
                        FinalAverage = finalAverage,
                        BancaCount = bancaCount,
                        BancaAverage = bancaAvg,
                        FeiraCount = feiraCount,
                        FeiraAverage = feiraAvg,
                        StudentCount = studentCount,
                        StudentAverage = studentAvg
                    });
                }
                catch
                {
                    // Ignorar projetos com erro no cálculo
                }
            }

            // Agrupar por média arredondada e aplicar desempate
            var random = new Random();
            var topProjects = projectRatingsIntermediate
                .GroupBy(p => p.FinalAverage)
                .OrderByDescending(g => g.Key)
                .SelectMany(g => g
                    .OrderByDescending(p => p.FeiraCount)
                    .ThenByDescending(p => p.StudentCount)
                    .ThenBy(p => random.Next())
                )
                .Take(5)
                .Select(p => new
                {
                    projectName = p.ProjectName,
                    finalAverage = p.FinalAverage,
                    assessments = new
                    {
                        banca = new
                        {
                            count = $"{p.BancaCount}/2",
                            average = p.BancaAverage
                        },
                        feira = new
                        {
                            count = $"{p.FeiraCount}/3",
                            average = p.FeiraAverage
                        },
                        student = new
                        {
                            count = $"{p.StudentCount}/3",
                            average = p.StudentAverage
                        }
                    }
                })
                .ToList<object>();

            return Results.Ok(topProjects);
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao buscar ranking de projetos: " + ex.Message);
        }
    }

    /// <summary>
    /// Retorna as notas finais dos projetos conforme filtros informados.
    /// </summary>
    /// <param name="filterDto">Filtros: semestre, curso, fullname e pontoMaterial.</param>
    /// <param name="http">HttpContext com token de autenticação.</param>
    /// <returns>Lista com notas finais dos projetos.</returns>
    [Authorize(Roles = "Teacher")]
    [HttpPost("/api/teacher/final-notes")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public static IResult GetFinalNotes([FromBody] FinalNotesFilterDto? filterDto, HttpContext http)
    {
        try
        {
            var teacher = GetAuthenticatedTeacher(http);
            if (teacher == null)
                return Results.NotFound("Professor não encontrado");

            filterDto ??= new FinalNotesFilterDto();

            var context = new EngenhariasSenacContext();

            // ============= OTIMIZAÇÃO 1: Filtrar e contar antes de carregar dados completos =============
            var studentsQuery = context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(filterDto.Fullname))
                studentsQuery = studentsQuery.Where(s => s.Fullname.ToLower().Contains(filterDto.Fullname.ToLower()));

            if (filterDto.Semester.HasValue)
                studentsQuery = studentsQuery.Where(s => s.Semester == filterDto.Semester.Value);

            if (!string.IsNullOrEmpty(filterDto.Course))
                studentsQuery = studentsQuery.Where(s => s.Course.ToLower() == filterDto.Course.ToLower());

            // ============= FILTRO DE POINTMATERIAL OTIMIZADO =============
            if (!string.IsNullOrEmpty(filterDto.PointMaterial))
            {
                var searchTerm = filterDto.PointMaterial.ToLower();
                
                // 1️⃣ Obter alunos que correspondem ao filtro no PointMaterial ou contêm "disciplina"
                var studentIdsFromPointMaterial = studentsQuery
                    .Where(s => 
                        s.PointMaterial.ToLower().Contains(searchTerm) || 
                        s.PointMaterial.ToLower().Contains("disciplina"))
                    .Select(s => s.CodStudents)
                    .ToList();

                // 2️⃣ Obter alunos que têm ExtraPoints com Discipline correspondente
                var studentIdsFromExtraPoints = context.ExtraPoints
                    .Where(e => !string.IsNullOrEmpty(e.Discipline) && 
                                (e.Discipline.ToLower().Contains(searchTerm) || 
                                 e.Discipline.ToLower().Contains("disciplina")))
                    .Select(e => e.StudentId)
                    .Distinct()
                    .ToList();

                // 3️⃣ Obter IDs dos líderes do comitê (eles sempre devem aparecer em qualquer filtro)
                var committeeLeaderIds = context.Committee
                    .Where(c => c.Role == "Leader")
                    .Select(c => c.IdSenac)
                    .ToList();

                // 4️⃣ Obter CodStudents dos líderes do comitê
                var committeeStudentIds = studentsQuery
                    .Where(s => committeeLeaderIds.Contains(s.IdSenac ?? 0))
                    .Select(s => s.CodStudents)
                    .ToList();

                // 5️⃣ Combinar os três resultados
                var combinedStudentIds = studentIdsFromPointMaterial
                    .Union(studentIdsFromExtraPoints)
                    .Union(committeeStudentIds)
                    .ToList();

                // Filtrar a query com os IDs combinados
                studentsQuery = studentsQuery.Where(s => combinedStudentIds.Contains(s.CodStudents));
            }

            var totalCount = studentsQuery.Count();

            // ============= OTIMIZAÇÃO 2: Carregar apenas dados necessários =============
            var studentIds = studentsQuery
                .OrderBy(s => s.Semester)
                .ThenBy(s => s.Course)
                .ThenBy(s => s.Fullname)
                .Skip((filterDto.Page - 1) * filterDto.PerPage)
                .Take(filterDto.PerPage)
                .Select(s => s.CodStudents)
                .ToList();

            var students = context.Students
                .Where(s => studentIds.Contains(s.CodStudents))
                .ToList();

            // ============= OTIMIZAÇÃO 3: Carregar TODOS os dados relacionados em 4 queries otimizadas =============
            var projectTeamIds = students
                .Where(s => s.ProjectTeam.HasValue)
                .Select(s => s.ProjectTeam!.Value)
                .Distinct()
                .ToList();

            // Query 1: ProjectNotes para todos os projetos
            var allProjectNotes = context.ProjectNotes
                .Where(p => projectTeamIds.Contains(p.EvaluatedProject))
                .ToList();

            // Query 2: ProjectAssessments para todos os projetos
            var allProjectAssessments = context.ProjectAssessment
                .Where(a => projectTeamIds.Contains(a.EvaluatedProject))
                .ToList();

            // Query 3: BusinessAssessments e ProjectAssessments por estudante
            var businessAssessmentCounts = context.BusinessAssessment
                .Where(a => students.Select(s => s.CodStudents).Contains(a.StudentEvaluator))
                .GroupBy(a => a.StudentEvaluator)
                .Select(g => new { StudentId = g.Key, Count = g.Count() })
                .ToList();

            var projectAssessmentCounts = context.ProjectAssessment
                .Where(a => students.Select(s => s.CodStudents).Contains(a.StudentEvaluator))
                .GroupBy(a => a.StudentEvaluator)
                .Select(g => new { StudentId = g.Key, Count = g.Count() })
                .ToList();

            // Query 4: LectureLogs apenas se necessário (para não-committee)
            var allLectures = context.Lectures.ToList();
            var lectureLogs = context.LectureLogs
                .Where(l => students.Select(s => s.CodStudents).Contains(l.Student))
                .ToList();

            // Query 5: ExtraPoints para todos os alunos
            var allExtraPoints = context.ExtraPoints
                .Where(e => students.Select(s => s.CodStudents).Contains(e.StudentId))
                .ToList();

            // ============= DADOS EM MEMÓRIA =============
            var registers = new List<object>();
            
            // Buscar IDs dos membros do comitê do banco de dados
            var committeeLeaders = context.Committee
                .Where(c => c.Role == "Leader")
                .Select(c => c.IdSenac)
                .ToList();
            
            var committeeParticipants = context.Committee
                .Where(c => c.Role == "Member")
                .Select(c => c.IdSenac)
                .ToList();


            // ============= OTIMIZAÇÃO 4: Processar dados em memória (agora com dados pré-carregados) =============
            foreach (var student in students)
            {
                if (!student.ProjectTeam.HasValue)
                    continue;

                var projectTeamId = student.ProjectTeam.Value;

                // Obter notas já carregadas
                var projectNotes = allProjectNotes
                    .Where(p => p.EvaluatedProject == projectTeamId)
                    .ToList();

                var studentAssessments = allProjectAssessments
                    .Where(a => a.EvaluatedProject == projectTeamId)
                    .ToList();

                var course = student.Course == "comp" ? "BEC" : student.Course == "prod" ? "BEP" : "Indefinido";

                // Separar notas por tipo
                var bancaNotes = projectNotes
                    .Where(n => n.EvaluateType == EvaluateType.Banca)
                    .ToList();

                var feiraNotes = projectNotes
                    .Where(n => n.EvaluateType == EvaluateType.Feira)
                    .ToList();

                // Calcular médias
                decimal bancaAvg = CalculateAverage(bancaNotes, 2);
                decimal feiraAvg = CalculateAverage(feiraNotes, 3);
                decimal studentAvg = CalculateStudentAverage(studentAssessments, 3);

                // Média final ponderada
                double finalAverage = Math.Round((double)((bancaAvg * 0.7m) + (feiraAvg * 0.25m) + (studentAvg * 0.05m)), 2);
                int reviewsNumber = projectNotes.Count;

                // Calcular extraNote
                double extraNote = 0;
                string pointMaterial = student.PointMaterial;

                int idSenac = student.IdSenac ?? 0;

                if (committeeLeaders.Contains(idSenac))
                {
                    extraNote = 1.0;
                    pointMaterial = "Todas as disciplinas - Comitê";
                }
                else if (committeeParticipants.Contains(idSenac))
                {
                    extraNote = 1.0;
                }
                else
                {
                    // Calcular presença e avaliações
                    int presentCount = CalculatePresentCount(student.CodStudents, allLectures, lectureLogs);
                    int businessCount = businessAssessmentCounts
                        .FirstOrDefault(b => b.StudentId == student.CodStudents)?.Count ?? 0;
                    int projectCount = projectAssessmentCounts
                        .FirstOrDefault(p => p.StudentId == student.CodStudents)?.Count ?? 0;

                    if (presentCount >= 4 && businessCount >= 5 && projectCount >= 5)
                        extraNote = 0.5;
                }

                // Buscar ExtraPoints do aluno
                var extraPoints = allExtraPoints
                    .Where(e => e.StudentId == student.CodStudents)
                    .Select(e => new
                    {
                        codExtraPoint = e.CodExtraPoint,
                        point = e.Point,
                        reason = e.Reason,
                        discipline = e.Discipline
                    })
                    .ToList();

                registers.Add(new
                {
                    idSenac = student.IdSenac,
                    studentName = student.Fullname,
                    semester = student.Semester,
                    pointMaterial,
                    reviewsNumber,
                    average = finalAverage,
                    extraNote,
                    extraPoints = extraPoints.Count > 0 ? extraPoints : null,
                    course
                });
            }

            // ============= SEMPRE ORDENA POR NOME ALFABETICAMENTE =============
            var sortedRegisters = registers
                .Cast<dynamic>()
                .OrderBy(r => r.studentName)
                .ToList<object>();

            return Results.Ok(new
            {
                total = totalCount,
                data = sortedRegisters
            });
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao buscar dados: " + ex.Message);
        }
    }

    /// <summary>
    /// Calcula a média de notas com divisor mínimo
    /// </summary>
    private static decimal CalculateAverage(List<ProjectNote> notes, int minDivisor)
    {
        if (notes.Count == 0)
            return 0;

        decimal sum = 0;
        foreach (var note in notes)
        {
            try
            {
                sum += ProjectRankingCalculator.ParseAssessmentPublic(note.Assessment);
            }
            catch
            {
                continue;
            }
        }

        int divisor = Math.Max(notes.Count, minDivisor);
        return Math.Round(sum / divisor, 2);
    }

    /// <summary>
    /// Calcula a média de avaliações de alunos
    /// </summary>
    private static decimal CalculateStudentAverage(List<ProjectAssessment> assessments, int minDivisor)
    {
        if (assessments.Count == 0)
            return 0;

        decimal sum = assessments.Sum(a => (decimal)a.Assessment);
        int divisor = Math.Max(assessments.Count, minDivisor);
        return Math.Round(sum / divisor, 2);
    }

    /// <summary>
    /// Calcula a contagem de presença com base em logs de aula
    /// </summary>
    private static int CalculatePresentCount(int studentId, List<Lecture> allLectures, List<LectureLog> allLectureLogs)
    {
        int presentCount = 0;

        foreach (var lecture in allLectures)
        {
            var logs = allLectureLogs
                .Where(l => l.Student == studentId &&
                            l.Room == lecture.Room &&
                            l.Datetime.Date == lecture.DatetimeStart.Date)
                .OrderBy(l => l.Datetime)
                .ToList();

            var stack = new Stack<DateTime>();
            double totalMinutesPresent = 0;

            foreach (var log in logs)
            {
                if (log.LogType == LogType.In)
                {
                    stack.Push(log.Datetime);
                }
                else if (log.LogType == LogType.Out && stack.Count > 0)
                {
                    var inTime = stack.Pop();
                    var outTime = log.Datetime;

                    if (inTime < lecture.DatetimeStart) inTime = lecture.DatetimeStart;
                    if (outTime > lecture.DatetimeEnd) outTime = lecture.DatetimeEnd;

                    if (inTime < outTime)
                        totalMinutesPresent += (outTime - inTime).TotalMinutes;
                }
            }

            double lectureMinutes = (lecture.DatetimeEnd - lecture.DatetimeStart).TotalMinutes;
            if (lectureMinutes > 0 && (totalMinutesPresent / lectureMinutes) * 100 >= 75)
            {
                presentCount++;
            }
        }

        return presentCount;
    }

    [Authorize(Roles = "Teacher")]
    [HttpGet("/api/teacher/point-materials")]
    public static IResult GetPointMaterials(HttpContext http)
    {
        try
        {
            var teacher = GetAuthenticatedTeacher(http);
            if (teacher == null)
                return Results.NotFound("Professor não encontrado");

            var context = new EngenhariasSenacContext();

            // Obter todos os PointMaterial únicos e não vazios
            var pointMaterials = context.Students
                .Where(s => !string.IsNullOrEmpty(s.PointMaterial))
                .Select(s => s.PointMaterial)
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            return Results.Ok(new
            {
                total = pointMaterials.Count,
                data = pointMaterials
            });
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao buscar pontos materiais: " + ex.Message);
        }
    }

    /// <summary>
    /// Classe intermediária para armazenar dados de rating para ranking
    /// </summary>
    private class ProjectRatingIntermediate
    {
        public string? ProjectName { get; set; }
        public decimal FinalAverage { get; set; }
        public int BancaCount { get; set; }
        public decimal BancaAverage { get; set; }
        public int FeiraCount { get; set; }
        public decimal FeiraAverage { get; set; }
        public int StudentCount { get; set; }
        public decimal StudentAverage { get; set; }
    }

}
