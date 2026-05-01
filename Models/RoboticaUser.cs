using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EngenhariasSenac.Models;

[Table("Users")]
[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Cpf), IsUnique = true)]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [ForeignKey("Auth")]
    public Guid AuthId { get; set; }

    public Guid? CodAutorizacao { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(150)]
    public string Name { get; set; } = null!;

    [MaxLength(14)]
    public string Cpf { get; set; } = null!;

    [MaxLength(150)]
    public string Email { get; set; } = null!;

    [MaxLength(50)]
    public string TypeUser { get; set; } = null!; // "Admin", "Professor", "Aluno"

    public Auth Auth { get; set; } = null!;
}

[Table("Auth")]
[Index(nameof(Email), IsUnique = true)]
public class Auth
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastLogin { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(150)]
    public string Email { get; set; } = null!;

    [MaxLength(255)]
    public string PasswordHash { get; set; } = null!;

    public ICollection<AuthRecovery> RecoveryCodes { get; set; } = new List<AuthRecovery>();
}

[Table("AuthRecovery")]
public class AuthRecovery
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [ForeignKey("Auth")]
    public Guid AuthId { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool Used { get; set; } = false;

    [MaxLength(10)]
    public string Code { get; set; } = null!;

    public Auth Auth { get; set; } = null!;
}
