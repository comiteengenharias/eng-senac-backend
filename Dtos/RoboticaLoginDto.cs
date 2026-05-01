namespace EngenhariasSenac.Dtos;

/// <summary>
/// Dados de login para Robótica.
/// </summary>
public class RoboticaLoginDto
{
    /// <summary>Endereço de e-mail do usuário.</summary>
    public string Email { get; set; } = null!;
    /// <summary>Senha do usuário.</summary>
    public string Password { get; set; } = null!;
}

/// <summary>
/// DTO para verificação 2FA.
/// </summary>
public class RoboticaVerifyDto
{
    /// <summary>Endereço de e-mail do usuário.</summary>
    public string Email { get; set; } = null!;
    /// <summary>Código de verificação 2FA recebido por e-mail.</summary>
    public string Code { get; set; } = null!;
}

/// <summary>
/// Dados necessários para registrar uma conta Robótica.
/// </summary>
public class RoboticaRegisterDto
{
    /// <summary>Endereço de e-mail do usuário.</summary>
    public string Email { get; set; } = null!;
    /// <summary>Senha do usuário.</summary>
    public string Password { get; set; } = null!;
    /// <summary>Nome completo do usuário.</summary>
    public string Name { get; set; } = null!;
    /// <summary>CPF do usuário.</summary>
    public string Cpf { get; set; } = null!;
    /// <summary>Tipo de usuário. Ex: "Robotica", "Admin".</summary>
    public string TypeUser { get; set; } = null!; // "Admin", "Professor", "Aluno"
}

/// <summary>
/// DTO para atualização de dados do usuário.
/// </summary>
public class RoboticaUpdateUserDto
{
    /// <summary>Nome completo.</summary>
    public string Name { get; set; } = null!;
    /// <summary>CPF.</summary>
    public string Cpf { get; set; } = null!;
}

/// <summary>
/// DTO para alteração de senha.
/// </summary>
public class RoboticaChangePasswordDto
{
    /// <summary>Senha atual do usuário.</summary>
    public string CurrentPassword { get; set; } = null!;
    /// <summary>Nova senha desejada.</summary>
    public string NewPassword { get; set; } = null!;
}

/// <summary>
/// DTO para criação de projeto Robótica (exemplo).
/// </summary>
public class CreateRoboticaProjectDto
{
    /// <summary>Nome do projeto.</summary>
    public string Name { get; set; } = null!;
    /// <summary>Descrição do projeto.</summary>
    public string Description { get; set; } = null!;
}
