using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EngenhariasSenac.Models;

[Table("Companies")]
public class Companies
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodCompany { get; set; }

    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string Description { get; set; } = null!;

    [MaxLength(255)]
    public string Picture { get; set; } = null!;
}
