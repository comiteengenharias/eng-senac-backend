namespace EngenhariasSenac.Dtos;

public class PlatformIssueDto
{
    public int StudentId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime SentAt { get; set; }
    public string? ImageUrl1 { get; set; }
    public string? ImageUrl2 { get; set; }
}
