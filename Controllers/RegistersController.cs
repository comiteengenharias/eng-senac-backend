using EngenhariasSenac.Banco;
using EngenhariasSenac.Database;
using EngenhariasSenac.Helpers;
using EngenhariasSenac.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using static EngenhariasSenac.Endpoints.RegistersEndpoints;

namespace EngenhariasSenac.Controllers;

public class RegistersController : ControllerBase
{
    /// <summary>
    /// Registra um novo líder de projeto (estudante) e cria o projeto associado.
    /// </summary>
    /// <param name="data">Objeto com `NewStudent` e `NewProject`.</param>
    /// <returns>Mensagem de sucesso ou erro.</returns>
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public static IResult PostLeaderRegistration([FromBody] LeaderRegistrationDto data)
    {
        var context = new EngenhariasSenacContext();

        var dalStudent = new DAL<Student>(context);
        var dalProject = new DAL<ProjectTeam>(context);

        try
        {
            // verifica duplicidade do id
            var verifyIdSenac = dalStudent.SelectWhere(a => a.IdSenac == data.NewStudent.IdSenac);
            if (verifyIdSenac is not null)
            {
                return Results.Conflict("ID do Senac já cadastrado");
            }

            // verifica duplicidade do e-mail
            var verifyEmail = dalStudent.SelectWhere(a => a.InstitutionalEmail == data.NewStudent.InstitutionalEmail);
            if (verifyEmail is not null)
            {
                return Results.Conflict("E-mail já cadastrado");
            }
            else if (!data.NewStudent.InstitutionalEmail.Contains("@senacsp.edu.br"))
            {
                return Results.Conflict("O e-mail institucional precisa conter @senacsp.edu.br");
            }

            // criptografa senha
            var hashedPsw = PasswordHasher.HashPassword(data.NewStudent.Password);
            data.NewStudent.Password = hashedPsw;

            // insere registro do estudante
            dalStudent.Insert(data.NewStudent);
            var insertedStudent = dalStudent.SelectWhere(a => a.IdSenac.Equals(data.NewStudent.IdSenac));

            // verifica se o estudente foi inserido com sucesso
            if (insertedStudent is not null)
            {
                // completa informações do projeto
                data.NewProject.Semester = insertedStudent.Semester;
                data.NewProject.Leader = insertedStudent.CodStudents;
                data.NewProject.Token = ProjectTokenGenerator.Generate();

                // verifica duplicidade do token
                while (dalProject.SelectWhere(a => a.Token == data.NewProject.Token) != null)
                {
                    data.NewProject.Token = ProjectTokenGenerator.Generate();
                }

                data.NewStudent.CodStudents = insertedStudent.CodStudents;
            }
            else
            {
                throw new Exception("Falha ao adicionar estudante");
            }

            // insere registro do projeto
            dalProject.Insert(data.NewProject);
            var insertedProject = dalProject.SelectWhere(a => a.Token.Equals(data.NewProject.Token));

            // verifica se o projeto foi inserido com sucesso
            if (insertedProject is not null)
            {
                data.NewStudent.ProjectTeam = insertedProject.CodTeam;
            }
            else
            {
                throw new Exception("Falha ao adicionar projeto");
            }

            // adiciona o código do projeto no registro do estudante
            dalStudent.Update(data.NewStudent);

            SendEmail.Send(
                insertedStudent.PersonalEmail ?? string.Empty,
                insertedStudent.InstitutionalEmail,
                "Cadastro como Líder realizado com sucesso",
                "<h1>Olá futuro(a) engenheiro</h1><p>Seu cadastro como líder da equipe " + insertedProject.GroupName + " foi realizada com sucesso.</p><p>O token da equipe é <strong>" + insertedProject.Token + "</strong>. Envie para seus colegas, e solicite o cadastramento como membro: https://engenhariasenac.com.br/cadastro/membro.</p><p>Qualquer dúvida estamos a disposição!</p><p>Nos vemos em breve ;)</p>");

            return Results.Ok("Usuário e projeto adicionados com sucesso");
        }
        catch (Exception ex)
        {
            dalStudent.Delete(data.NewStudent);
            dalProject.Delete(data.NewProject);
            return Results.Problem("Houve um erro: " + ex.Message);
        }
    }

    /// <summary>
    /// Registra um novo membro de projeto usando token do grupo.
    /// </summary>
    /// <param name="data">Objeto com `NewStudent` e `Token`.</param>
    /// <returns>Mensagem de sucesso ou erro.</returns>
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public static IResult PostMemberRegistration([FromBody] MemberRegistrationDto data)
    {
        try
        {

            var context = new EngenhariasSenacContext();

            var dalStudent = new DAL<Student>(context);
            var dalProject = new DAL<ProjectTeam>(context);

            // verifica duplicidade do id
            var verifyIdSenac = dalStudent.SelectWhere(a => a.IdSenac == data.NewStudent.IdSenac);
            if (verifyIdSenac is not null)
            {
                return Results.Conflict("ID do Senac já cadastrado");
            }

            // verifica duplicidade do e-mail
            var verifyEmail = dalStudent.SelectWhere(a => a.InstitutionalEmail == data.NewStudent.InstitutionalEmail);
            if (verifyEmail is not null)
            {
                return Results.Conflict("E-mail já cadastrado");
            }
            else if (!data.NewStudent.InstitutionalEmail.Contains("@senacsp.edu.br"))
            {
                return Results.Conflict("O e-mail institucional precisa conter @senacsp.edu.br");
            }

            // verifica token do grupo
            var verifyToken = dalProject.SelectWhere(a => a.Token == data.Token);
            if (verifyToken is not null)
            {
                // define FK ProjectTeam em Students
                data.NewStudent.ProjectTeam = verifyToken.CodTeam;
            }
            else
            {
                return Results.Conflict("Grupo informado não existe");
            }

            // criptografa senha
            var hashedPsw = PasswordHasher.HashPassword(data.NewStudent.Password);
            data.NewStudent.Password = hashedPsw;

            // insere registro do estudante
            dalStudent.Insert(data.NewStudent);
            var insertedStudent = dalStudent.SelectWhere(a => a.IdSenac.Equals(data.NewStudent.IdSenac));

            SendEmail.Send(
                insertedStudent!.PersonalEmail ?? string.Empty,
                insertedStudent.InstitutionalEmail,
                "Cadastro como Membro realizado com sucesso",
                "<h1>Olá futuro(a) engenheiro</h1><p>Seu cadastro como membro da equipe " + verifyToken.GroupName + " foi realizada com sucesso.</p><p>Qualquer dúvida estamos a disposição!</p><p>Nos vemos em breve ;)</p>");

            return Results.Ok("Usuário cadastrado com sucesso");

        }
        catch (Exception ex)
        {
            return Results.Problem("Houve um erro: " + ex.Message);
        }
    }

    /// <summary>
    /// Registra um novo professor (validação por token permitido) e envia e-mail de confirmação.
    /// </summary>
    /// <param name="data">Objeto com `NewTeacher` e `Token`.</param>
    /// <returns>Mensagem de sucesso ou erro.</returns>
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public static IResult PostTeacherRegistration([FromBody] TeacherRegistrationDto data)
    {
        try
        {
            var context = new EngenhariasSenacContext();
            var dalTeacher = new DAL<Teacher>(context);

            // Lista de tokens válidos (IDs dos professores)
            var allowedTokens = new List<string>
            {
                "1140009839", "1140011773", "1140107674", "1142879180", "1142505514",
                "1140952511", "1140121266", "1142657941", "1143233197", "1140011920",
                "1140104453", "1140011995", "1141949898", "1143010566", "1141565032",
                "1140123180", "1140012173", "1140753442", "1142873697", "1140060076",
                "1143008405", "1141371511", "1140012251", "1141559148", "1140137438",
                "1142991647", "1140108435", "1140012392", "1140147748", "1140563248",
                "1142644472", "1141778940", "1143252886", "1140012459", "1140138230",
                "1142977906"
            };

            var teacherAdminToken = Environment.GetEnvironmentVariable("TEACHER_ADMIN_TOKEN");
            if (!string.IsNullOrEmpty(teacherAdminToken))
                allowedTokens.Add(teacherAdminToken);

            // Verifica duplicidade do e-mail
            var verifyEmail = dalTeacher.SelectWhere(a => a.InstitutionalEmail == data.NewTeacher.InstitutionalEmail);
            if (verifyEmail is not null)
            {
                return Results.Conflict("E-mail já cadastrado");
            }
            else if (!data.NewTeacher.InstitutionalEmail.Contains("senac"))
            {
                return Results.Conflict("O e-mail institucional precisa conter o domínio do senac");
            }

            // Verifica se o token está na lista de tokens permitidos
            if (!allowedTokens.Contains(data.Token))
            {
                return Results.Conflict("ID de professor inválido");
            }

            // Criptografa a senha
            var hashedPsw = PasswordHasher.HashPassword(data.NewTeacher.Password);
            data.NewTeacher.Password = hashedPsw;

            // Insere registro do professor
            dalTeacher.Insert(data.NewTeacher);

            // Envia e-mail de confirmação
            SendEmail.Send(
                "",
                data.NewTeacher.InstitutionalEmail,
                "Cadastro como Professor realizado com sucesso",
                "<h1>Olá professor(a)</h1><p>Seu cadastro na plataforma Engenharias Senac foi realizado com sucesso.</p><p>Estamos felizes em contar com a sua participação!</p><p>Nos vemos em breve ;)</p>"
            );

            return Results.Ok("Usuário cadastrado com sucesso");
        }
        catch (Exception ex)
        {
            return Results.Problem("Houve um erro: " + ex.Message);
        }
    }


}
