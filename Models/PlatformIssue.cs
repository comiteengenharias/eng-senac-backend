using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EngenhariasSenac.Models;

[Table("PlatformIssues")]
public class PlatformIssue
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodIssue { get; set; }

    [ForeignKey("Student")]
    public int StudentId { get; set; }

    [MaxLength(255)]
    public string Title { get; set; } = null!;

    [MaxLength(2000)]
    public string Description { get; set; } = null!;

    public DateTime SentAt { get; set; }

    [MaxLength(1000)]
    public string? ImageUrl1 { get; set; }

    [MaxLength(1000)]
    public string? ImageUrl2 { get; set; }

    public bool Checked { get; set; } = false;

    [MaxLength(2000)]
    public string? ResolutionComment { get; set; }

    public Student? Student { get; set; }
}
