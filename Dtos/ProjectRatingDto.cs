namespace EngenhariasSenac.Dtos;

/// <summary>
/// DTO para retornar a avaliação de uma categoria específica de avaliação
/// </summary>
public class ProjectRatingDto
{
    /// <summary>
    /// Quantidade de avaliações recebidas / Quantidade mínima de avaliações
    /// Exemplo: "2/2"
    /// </summary>
    public string AssessmentCount { get; set; } = null!;

    /// <summary>
    /// Média das avaliações da categoria
    /// </summary>
    public decimal Average { get; set; }
}

