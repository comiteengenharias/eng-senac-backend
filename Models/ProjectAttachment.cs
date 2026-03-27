using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EngenhariasSenac.Models;

[Table("ProjectAttachment")]
public class ProjectAttachment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodAttachment { get; set; }

    [MaxLength(255)]
    public string Path { get; set; } = null!;

    [MaxLength(255)]
    public string? Description { get; set; }

    [ForeignKey("ProjectTeam")]
    public int? ProjectTeam { get; set; }
}
