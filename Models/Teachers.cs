using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EngenhariasSenac.Models;

[Table("Teachers")]
[Index(nameof(InstitutionalEmail), IsUnique = true)]
public class Teacher
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodTeacher { get; set; }

    [MaxLength(255)]
    public string Fullname { get; set; } = null!;

    [MaxLength(255)]
    public string InstitutionalEmail { get; set; } = null!;

    [MaxLength(255)]
    public string Password { get; set; } = null!;
}
