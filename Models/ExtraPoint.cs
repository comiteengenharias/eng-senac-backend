using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EngenhariasSenac.Models;

[Table("ExtraPoints")]
public class ExtraPoint
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodExtraPoint { get; set; }

    [ForeignKey("Student")]
    public int StudentId { get; set; }

    [Precision(18, 2)]
    public decimal Point { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }

    [MaxLength(255)]
    public string? Discipline { get; set; }

    public Student? Student { get; set; }
}

