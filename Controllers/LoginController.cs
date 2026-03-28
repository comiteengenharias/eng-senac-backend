using Azure;
using EngenhariasSenac.Banco;
using EngenhariasSenac.Database;
using EngenhariasSenac.Helpers;
using EngenhariasSenac.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static EngenhariasSenac.Endpoints.LoginEndpoints;


namespace EngenhariasSenac.Controllers;

public class LoginController : ControllerBase
{
    [HttpGet("/api/login/student")]
    public static IResult PostStudentLogin([FromBody] StudentLoginDto data, HttpResponse response)
    {
        var context = new EngenhariasSenacContext();
        var dalStudent = new DAL<Student>(context);

        try
        {
            var userData = dalStudent.SelectWhere(a => a.InstitutionalEmail == data.Email);

            // verifica se o cadastro existe ou se a senha está incorreta
            if (userData == null || !PasswordHasher.VerifyPassword(data.Password, userData.Password))
                return Results.Conflict("Credenciais inválidas. Verifique o e-mail e a senha.");

            // Gera tokens
            var (accessToken, refreshToken) = TokenService.GenerateTokens(userData.InstitutionalEmail, "Student", userData.CodStudents);

            response.Cookies.Append("jwt", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // use somente se for HTTPS
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            });

            return Results.Ok("Login realizado com sucesso");
        }
        catch (Exception ex)
        {
            return Results.Problem("Houve um erro: " + ex.Message);
        }
    }

    [HttpGet("/api/login/teacher")]
    public static IResult PostTeacherLogin([FromBody] TeacherLoginDto data, HttpResponse response)
    {
        try
        {
            var context = new EngenhariasSenacContext();
            var dalTeacher = new DAL<Teacher>(context);
            var userData = dalTeacher.SelectWhere(a => a.InstitutionalEmail == data.Email);

            // verifica se os dados são válidos (not null)
            if (string.IsNullOrWhiteSpace(data.Email) || string.IsNullOrWhiteSpace(data.Password))
                return Results.BadRequest("E-mail e senha são obrigatórios.");

            // verifica se o cadastro existe ou se a senha está incorreta
            if (userData == null || !PasswordHasher.VerifyPassword(data.Password, userData.Password))
                return Results.Conflict("Credenciais inválidas. Verifique o e-mail e a senha.");

            // Gera tokens
            var (accessToken, refreshToken) = TokenService.GenerateTokens(userData.InstitutionalEmail, "Teacher", userData.CodTeacher);

            response.Cookies.Append("jwt", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            });

            return Results.Ok("Login realizado com sucesso");
        }
        catch (Exception ex)
        {
            return Results.Problem("Houve um erro: " + ex.Message);
        }
    }

    [HttpGet("/api/login/support")]
    public static IResult PostSupportLogin([FromBody] SupportLoginDto data, HttpResponse response)
    {
        try
        {
            // verifica a senha
            var supportPassword = Environment.GetEnvironmentVariable("SUPPORT_PASSWORD") ?? throw new InvalidOperationException("SUPPORT_PASSWORD não definido no .env");
            if (data.Password != supportPassword)
                return Results.Conflict("Credenciais inválidas. Verifique o e-mail e a senha.");

            // Gera tokens
            var (accessToken, refreshToken) = TokenService.GenerateTokens("apoio", "Support", 0);

            response.Cookies.Append("jwt", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // use somente se for HTTPS
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(5)
            });

            return Results.Ok("Login realizado com sucesso");
        }
        catch (Exception ex)
        {
            return Results.Problem("Houve um erro: " + ex.Message);
        }
    }

    [HttpGet("/api/verify-login")]
    public static IResult PostVerifyLogin(HttpRequest request)
    {
        try
        {
            var token = request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
                return Results.Unauthorized();

            var principal = TokenService.ValidateToken(token);
            if (principal == null)
                return Results.Unauthorized();

            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value;
            var userId = principal.FindFirst("UserId")?.Value;

            if (email == null && (role == "student" || role == "teacher"))
                return Results.Unauthorized();

            return Results.Ok(new
            {
                loggedIn = true,
                role,
                email,
                userId
            });
        }
        catch
        {
            return Results.Unauthorized();
        }
    }

    [HttpPost("/api/logout")]
    public static IResult PostLogout(HttpResponse response)
    {
        response.Cookies.Append("jwt", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(-1) // expira no passado
        });

        return Results.Ok("Logout realizado com sucesso");
    }

    [HttpPost("/api/recover-password/teacher")]
    public static IResult RecoverTeacherPassword([FromBody] string institutionalEmail)
    {
        try
        {
            var context = new EngenhariasSenacContext();
            var dalTeacher = new DAL<Teacher>(context);

            // Verifica se o e-mail existe
            var teacher = dalTeacher.SelectWhere(t => t.InstitutionalEmail == institutionalEmail);
            if (teacher == null)
                return Results.NotFound("E-mail não cadastrado");

            // Gera nova senha aleatória de 8 caracteres
            var newPassword = PasswordGenerator.Generate(8);

            // Atualiza a senha (hash)
            teacher.Password = PasswordHasher.HashPassword(newPassword);
            dalTeacher.Update(teacher);

            // Envia a nova senha por e-mail
            SendEmail.Send(
                teacher.InstitutionalEmail,
                teacher.InstitutionalEmail,
                "Recuperação de Senha - Engenharias Senac",
                $"<p>Olá, professor(a) {teacher.Fullname}!</p><p>Sua nova senha é: <strong>{newPassword}</strong></p><p>Recomendamos que altere essa senha após o primeiro login.</p><p>Atenciosamente, Comitê das Engenharias Senac</p>"
            );

            return Results.Ok("Nova senha enviada para seu e-mail.");
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao recuperar senha: " + ex.Message);
        }
    }

    [HttpPost("/api/recover-password/student")]
    public static IResult RecoverStudentPassword([FromBody] string institutionalEmail)
    {
        try
        {
            var context = new EngenhariasSenacContext();
            var dalStudent = new DAL<Student>(context);

            // Verifica se o e-mail existe
            var student = dalStudent.SelectWhere(s => s.InstitutionalEmail == institutionalEmail);
            if (student == null)
                return Results.NotFound("E-mail não cadastrado");

            // Gera nova senha aleatória de 8 caracteres
            var newPassword = PasswordGenerator.Generate(8);

            // Atualiza a senha (hash)
            student.Password = PasswordHasher.HashPassword(newPassword);
            dalStudent.Update(student);

            // Envia a nova senha para o e-mail institucional e pessoal
            SendEmail.Send(
                student.PersonalEmail ?? student.InstitutionalEmail,
                student.InstitutionalEmail,
                "Recuperação de Senha - Engenharias Senac",
                $"<p>Olá, {student.Fullname}!</p><p>Sua nova senha é: <strong>{newPassword}</strong></p><p>Recomendamos que altere essa senha após o primeiro login.</p><p>Atenciosamente, Comitê das Engenharias Senac</p>"
            );

            return Results.Ok("Nova senha enviada para seu e-mail.");
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao recuperar senha: " + ex.Message);
        }
    }





}
