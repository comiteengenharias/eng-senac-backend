using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EngenhariasSenac.Models;

[Table("Lectures")]
public class Lecture
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodLecture { get; set; }

    [ForeignKey("LectureSpeakers")]
    public int Speaker { get; set; }

    public DateTime DatetimeStart { get; set; }

    public DateTime DatetimeEnd { get; set; }

    [MaxLength(10)]
    public string Room { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    [MaxLength(255)]
    public string Picture { get; set; } = null!;

    public bool Checked { get; set; } = false;
}
