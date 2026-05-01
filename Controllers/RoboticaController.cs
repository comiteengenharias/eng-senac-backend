using EngenhariasSenac.Banco;
using EngenhariasSenac.Database;
using EngenhariasSenac.Models;
using EngenhariasSenac.Dtos;
using EngenhariasSenac.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EngenhariasSenac.Controllers;

[Authorize]
[ApiController]
[Route("api/robotica")]
public class RoboticaController : ControllerBase
{
    public static User? GetAuthenticatedUser(HttpContext http)
    {
        try
        {
            var emailClaim = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? http.User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(emailClaim))
                return null;

            var context = new EngenhariasSenacContext();
            var dal = new DAL<User>(context);
            return dal.SelectWhere(u => u.Email == emailClaim);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Retorna as informações do usuário autenticado.
    /// </summary>
    /// <returns>Dados do usuário (id, name, email, cpf, typeUser, createdAt).</returns>
    [HttpGet("me")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public static IResult GetUserInfo(HttpContext http)
    {
        var user = GetAuthenticatedUser(http);
        if (user == null)
            return Results.NotFound(new { message = "Usuário não encontrado" });

        return Results.Ok(new
        {
            id = user.Id,
            name = user.Name,
            email = user.Email,
            cpf = user.Cpf,
            typeUser = user.TypeUser,
            createdAt = user.CreatedAt
        });
    }

    /// <summary>
    /// Atualiza nome e CPF do usuário autenticado.
    /// </summary>
    /// <param name="data">Objeto com `name` e `cpf`.</param>
    /// <param name="http">Contexto HTTP da requisição (token de autenticação).</param>
    /// <returns>Mensagem de sucesso ou erro.</returns>
    [HttpPut("me")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public static IResult UpdateUser([FromBody] RoboticaUpdateUserDto data, HttpContext http)
    {
        var user = GetAuthenticatedUser(http);
        if (user == null)
            return Results.NotFound(new { message = "Usuário não encontrado" });

        try
        {
            var context = new EngenhariasSenacContext();
            var dal = new DAL<User>(context);

            user.Name = data.Name;
            user.Cpf = data.Cpf;

            dal.Update(user);
            context.SaveChanges();

            return Results.Ok(new { message = "Dados atualizados com sucesso" });
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao atualizar dados: " + ex.Message);
        }
    }

    /// <summary>
    /// Altera a senha do usuário autenticado.
    /// </summary>
    /// <param name="data">Objeto com `currentPassword` e `newPassword`.</param>
    /// <param name="http">Contexto HTTP da requisição (token de autenticação).</param>
    /// <returns>Mensagem de sucesso ou conflito caso a senha atual esteja incorreta.</returns>
    [HttpPut("password")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public static IResult ChangePassword([FromBody] RoboticaChangePasswordDto data, HttpContext http)
    {
        var user = GetAuthenticatedUser(http);
        if (user == null)
            return Results.NotFound(new { message = "Usuário não encontrado" });

        try
        {
            var context = new EngenhariasSenacContext();
            var dalAuth = new DAL<Auth>(context);
            var auth = dalAuth.SelectWhere(a => a.Email == user.Email);

            if (auth == null)
                return Results.NotFound(new { message = "Registro de autenticação não encontrado" });

            // Valida senha atual
            if (!PasswordHasher.VerifyPassword(data.CurrentPassword, auth.PasswordHash))
                return Results.Conflict(new { message = "Senha atual incorreta" });

            // Atualiza para nova senha
            auth.PasswordHash = PasswordHasher.HashPassword(data.NewPassword);
            dalAuth.Update(auth);
            context.SaveChanges();

            return Results.Ok(new { message = "Senha alterada com sucesso" });
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao alterar senha: " + ex.Message);
        }
    }

    /// <summary>
    /// Deleta a conta do usuário autenticado (Auth e User records).
    /// </summary>
    /// <returns>Mensagem de sucesso ou erro.</returns>
    [HttpDelete("me")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public static IResult DeleteAccount(HttpContext http)
    {
        var user = GetAuthenticatedUser(http);
        if (user == null)
            return Results.NotFound(new { message = "Usuário não encontrado" });

        try
        {
            var context = new EngenhariasSenacContext();
            var dalUser = new DAL<User>(context);
            var dalAuth = new DAL<Auth>(context);

            var auth = dalAuth.SelectWhere(a => a.Id == user.AuthId);
            if (auth != null)
            {
                dalAuth.Delete(auth);
            }

            dalUser.Delete(user);
            context.SaveChanges();

            return Results.Ok(new { message = "Conta deletada com sucesso" });
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao deletar conta: " + ex.Message);
        }
    }
}

