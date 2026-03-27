using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EngenhariasSenac.Models;

[Table("Committee")]
[Index(nameof(IdSenac), IsUnique = true)]
public class Committee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodId { get; set; }

    public int IdSenac { get; set; }

    [MaxLength(50)]
    public string Role { get; set; } = null!;
}