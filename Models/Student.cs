using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EngenhariasSenac.Models;

[Table("Students")]
[Index(nameof(IdSenac), IsUnique = true)]
[Index(nameof(InstitutionalEmail), IsUnique = true)]
public class Student
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodStudents { get; set; }

    public int? IdSenac { get; set; }

    [MaxLength(255)]
    public string Fullname { get; set; } = null!;

    [MaxLength(255)]
    public string InstitutionalEmail { get; set; } = null!;

    [MaxLength(255)]
    public string? PersonalEmail { get; set; }

    [MaxLength(255)]
    public string Cellphone { get; set; } = null!;

    [ForeignKey("ProjectTeam")]
    public int? ProjectTeam { get; set; }

    [MaxLength(255)]
    public string Password { get; set; } = null!;

    public int Semester { get; set; }

    [MaxLength(4)]
    public string Course { get; set; } = null!;

    [MaxLength(255)]
    public string PointMaterial { get; set; } = null!;
}
