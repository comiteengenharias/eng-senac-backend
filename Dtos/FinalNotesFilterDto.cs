namespace EngenhariasSenac.Dtos;

public class FinalNotesFilterDto
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 25;
    public string? Fullname { get; set; }
    public int? Semester { get; set; }
    public string? Course { get; set; }
    public string? PointMaterial { get; set; }
}

