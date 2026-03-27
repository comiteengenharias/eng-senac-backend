using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EngenhariasSenac.Models;

[Table("LectureLogs")]
public class LectureLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CodLog { get; set; }

    [ForeignKey("Student")]
    public int Student { get; set; }

    public DateTime Datetime { get; set; }

    [MaxLength(255)]
    public string Room { get; set; } = null!;

    [Required]
    public LogType LogType { get; set; }

}

public enum LogType
{
    In,
    Out
}
