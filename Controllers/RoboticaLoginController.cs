using EngenhariasSenac.Banco;
using EngenhariasSenac.Database;
using EngenhariasSenac.Models;
using EngenhariasSenac.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EngenhariasSenac.Dtos;
using System.Security.Claims;

namespace EngenhariasSenac.Controllers;

public class RoboticaLoginController
{
    // Armazena temporariamente o código 2FA (em produção, usar cache ou banco)
    private static Dictionary<string, (string code, DateTime expiry)> TwoFACodes = new();

    /// <summary>
    /// Envia código 2FA para o e-mail do usuário após validação das credenciais.
    /// </summary>
    /// <param name="data">Objeto com email e senha do usuário.</param>
    /// <returns>Mensagem indicando que o código foi enviado e que a verificação é necessária.</returns>
    [HttpPost("/api/robotica/login")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public static IResult PostLogin([FromBody] RoboticaLoginDto data)
    {
        try
        {
            var context = new EngenhariasSenacContext();
            var dal = new DAL<Auth>(context);

            var auth = dal.SelectWhere(u => u.Email == data.Email);
            if (auth == null || !PasswordHasher.VerifyPassword(data.Password, auth.PasswordHash))
                return Results.Conflict(new { message = "Credenciais inválidas" });

            // Gera código 2FA (6 dígitos)
            var twoFACode = new Random().Next(100000, 999999).ToString();
            TwoFACodes[auth.Email] = (twoFACode, DateTime.UtcNow.AddMinutes(5));

            // Enviar Código 2FA para o email do usuário
            SendEmail.Send(auth.Email, auth.Email, "Seu código de verificação - 2FA", 
                $"<h1>Seu código de verificação</h1><p>Código: <strong>{twoFACode}</strong></p><p>Este código expira em 5 minutos.</p>");

            return Results.Ok(new
            {
                message = "Código de verificação enviado",
                requiresVerification = true
            });
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro: " + ex.Message);
        }
    }

    /// <summary>
    /// Verifica o código 2FA enviado ao e-mail e emite token JWT em cookie.
    /// </summary>
    /// <param name="data">Objeto com email e código de verificação.</param>
    /// <param name="response">Resposta HTTP para anexar cookie com o token.</param>
    /// <returns>Token de acesso se a verificação for bem-sucedida.</returns>
    [HttpPost("/api/robotica/verify")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public static IResult PostVerify([FromBody] RoboticaVerifyDto data, HttpResponse response)
    {
        try
        {
            // Valida código 2FA
            if (!TwoFACodes.ContainsKey(data.Email) || 
                TwoFACodes[data.Email].code != data.Code ||
                TwoFACodes[data.Email].expiry < DateTime.UtcNow)
            {
                return Results.Conflict(new { message = "Código inválido ou expirado" });
            }

            var context = new EngenhariasSenacContext();
            var dal = new DAL<Auth>(context);
            var auth = dal.SelectWhere(u => u.Email == data.Email);

            if (auth == null)
                return Results.NotFound(new { message = "Usuário não encontrado" });

            // Remove código utilizado
            TwoFACodes.Remove(data.Email);

            // Atualiza last_login
            auth.LastLogin = DateTime.UtcNow;
            context.Auths.Update(auth);
            context.SaveChanges();

            // Gera token JWT
            var (accessToken, refreshToken) = TokenService.GenerateTokens(auth.Email, "Robotica", Math.Abs(auth.Id.GetHashCode()));

            response.Cookies.Append("jwt", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            });

            return Results.Ok(new
            {
                message = "Verificação concluída com sucesso",
                token = accessToken
            });
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro: " + ex.Message);
        }
    }

    /// <summary>
    /// Cria uma nova conta de usuário no sistema.
    /// </summary>
    /// <param name="data">Dados de registro: email, senha, nome, CPF e tipo de usuário.</param>
    /// <returns>Informação da criação do usuário.</returns>
    [HttpPost("/api/robotica/register")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public static IResult PostRegister([FromBody] RoboticaRegisterDto data)
    {
        try
        {
            var context = new EngenhariasSenacContext();
            var dalAuth = new DAL<Auth>(context);
            var dalUser = new DAL<User>(context);

            // Verifica se email já existe
            var existingAuth = dalAuth.SelectWhere(a => a.Email == data.Email);
            if (existingAuth != null)
                return Results.Conflict(new { message = "Email já cadastrado" });

            // Valida dados obrigatórios
            if (string.IsNullOrWhiteSpace(data.Email) || string.IsNullOrWhiteSpace(data.Password) ||
                string.IsNullOrWhiteSpace(data.Name) || string.IsNullOrWhiteSpace(data.Cpf))
                return Results.BadRequest(new { message = "Todos os campos são obrigatórios" });

            // Cria registro de autenticação
            var passwordHash = PasswordHasher.HashPassword(data.Password);
            var auth = new Auth
            {
                Id = Guid.NewGuid(),
                Email = data.Email,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            dalAuth.Insert(auth);

            // Cria registro de usuário
            var user = new User
            {
                Id = Guid.NewGuid(),
                AuthId = auth.Id,
                Email = data.Email,
                Name = data.Name,
                Cpf = data.Cpf,
                TypeUser = data.TypeUser,
                CreatedAt = DateTime.UtcNow
            };

            dalUser.Insert(user);

            return Results.Created("/auth/me", new
            {
                message = "Conta criada com sucesso",
                userId = user.Id
            });
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao criar conta: " + ex.Message);
        }
    }
}
