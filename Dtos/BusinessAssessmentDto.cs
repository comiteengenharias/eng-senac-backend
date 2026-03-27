namespace EngenhariasSenac.Dtos;

public class BusinessAssessmentDto
{
    public int CompanyId { get; set; }
    public decimal Assessment { get; set; }
    public string Comment { get; set; } = null!;
}
