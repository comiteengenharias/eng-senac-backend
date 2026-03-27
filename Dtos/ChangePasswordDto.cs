namespace EngenhariasSenac.Dtos;

public class ChangePasswordDto
{
    public string CurrentPsw { get; set; } = null!;
    public string NewPsw { get; set; } = null!;
}