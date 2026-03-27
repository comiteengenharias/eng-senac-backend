using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EngenhariasSenac.Models;

[Table("ProjectAssessments")]
public class ProjectAssessment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodAssessment { get; set; }

    // Chave estrangeira para o aluno que está avaliando
    [ForeignKey("StudentEvaluator")]
    public int StudentEvaluator { get; set; }

    // Chave estrangeira para o projeto avaliado
    [ForeignKey("EvaluatedProject")]
    public int EvaluatedProject { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Assessment { get; set; }

    [MaxLength(255)]
    public string Picture { get; set; } = null!;

    [MaxLength(500)]
    public string Comment { get; set; } = null!;

}
