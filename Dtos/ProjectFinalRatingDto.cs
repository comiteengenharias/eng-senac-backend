namespace EngenhariasSenac.Dtos;

/// <summary>
/// DTO para retornar os dados finais de avaliação de um projeto
/// </summary>
public class ProjectFinalRatingDto
{
    /// <summary>
    /// Avaliação de professor em banca (Peso 70%)
    /// </summary>
    public ProjectRatingDto BancaRating { get; set; } = null!;

    /// <summary>
    /// Avaliação de professor em feira (Peso 25%)
    /// </summary>
    public ProjectRatingDto FeeraRating { get; set; } = null!;

    /// <summary>
    /// Avaliação de aluno (Peso 5%)
    /// </summary>
    public ProjectRatingDto StudentRating { get; set; } = null!;

    /// <summary>
    /// Média ponderada final
    /// Cálculo: (BancaAverage * 0.7) + (FeeraAverage * 0.25) + (StudentAverage * 0.05)
    /// </summary>
    public decimal FinalAverage { get; set; }

    /// <summary>
    /// Posição no ranking do grupo dentro do semestre
    /// </summary>
    public int RankingPosition { get; set; }

    /// <summary>
    /// Média ponderada final para fins de ranking
    /// </summary>
    public decimal RankingAverage { get; set; }
}

