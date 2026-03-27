using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EngenhariasSenac.Models;

[Table("ProjectTeams")]
[Index(nameof(Token), IsUnique = true)]
public class ProjectTeam
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodTeam { get; set; }

    [MaxLength(10)]
    public string Token { get; set; } = null!;

    [MaxLength(255)]
    public string GroupName { get; set; } = null!;

    [MaxLength(255)]
    public int Semester { get; set; }

    public string Description { get; set; } = null!;

    [ForeignKey("Leader")]
    public int? Leader { get; set; }

}
