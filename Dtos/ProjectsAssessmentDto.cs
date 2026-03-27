namespace EngenhariasSenac.Dtos;

public class ProjectsAssessmentDto
{
    public int ProjectId { get; set; }
    public decimal Assessment { get; set; }
    public string Comment { get; set; } = null!;
}
