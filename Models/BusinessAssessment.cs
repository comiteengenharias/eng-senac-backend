using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EngenhariasSenac.Models;

[Table("BusinessAssessment")]
public class BusinessAssessment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodAssessment { get; set; }

    [ForeignKey("StudentEvaluatorNavigation")]
    public int StudentEvaluator { get; set; }

    [ForeignKey("CompanyEvaluatedNavigation")]
    public int CompanyEvaluated { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Assessment { get; set; }

    [MaxLength(500)]
    public string Comment { get; set; } = null!;

    [MaxLength(255)]
    public string Picture { get; set; } = null!;

}
