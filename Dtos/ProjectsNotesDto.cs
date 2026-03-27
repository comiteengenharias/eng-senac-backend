using EngenhariasSenac.Models;

namespace EngenhariasSenac.Dtos;

public class ProjectsNotesDto
{
    public int ProjectId { get; set; }
    public string Assessment { get; set; } = null!;
    public string Comment { get; set; } = null!;
    public EvaluateType EvaluateType { get; set; }
}
