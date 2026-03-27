using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EngenhariasSenac.Models;

[Table("LectureSpeakers")]
public class LectureSpeaker
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodSpeaker { get; set; }

    [MaxLength(255)]
    public string Fullname { get; set; } = null!;

    [MaxLength(255)]
    public string Linkedin { get; set; } = null!;
}
