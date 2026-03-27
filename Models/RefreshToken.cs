namespace EngenhariasSenac.Models;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = null!;
    public string Username { get; set; } = null!;
    public DateTime ExpirationDate { get; set; }
    public bool IsRevoked { get; set; }
}
