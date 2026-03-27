using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EngenhariasSenac.Models;

[Table("ProjectNotes")]
public class ProjectNote
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodAssessment { get; set; }

    // Chave estrangeira para o professor avaliador
    [ForeignKey("TeacherEvaluator")]
    public int TeacherEvaluator { get; set; }

    // Chave estrangeira para o projeto avaliado
    [ForeignKey("EvaluatedProject")]
    public int EvaluatedProject { get; set; }

    [Column(TypeName = "nvarchar(MAX)")]
    public string Assessment { get; set; } = null!;

    [MaxLength(500)]
    public string Comment { get; set; } = null!;

    public DateTime Datetime { get; set; }

    [Required]
    [Column(TypeName = "varchar(10)")]
    [EnumDataType(typeof(EvaluateType))]
    public EvaluateType EvaluateType { get; set; }
}

public enum EvaluateType
{
    Feira,
    Banca
}

// Feira = 0
// Banca = 1
