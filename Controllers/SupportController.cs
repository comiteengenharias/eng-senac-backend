using EngenhariasSenac.Banco;
using EngenhariasSenac.Database;
using EngenhariasSenac.Helpers;
using EngenhariasSenac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static EngenhariasSenac.Endpoints.SupportEndpoints;

namespace EngenhariasSenac.Controllers;

public class SupportController : ControllerBase
{

    [Authorize(Roles = "Support")]
    public static IResult GetAllRooms(HttpContext http)
    {
        try
        {
            var context = new EngenhariasSenacContext();

            var dalLectures = new DAL<Lecture>(context);
            var rooms = dalLectures
                .Select()
                .Select(l => l.Room)
                .Distinct()
                .ToList();

            return Results.Ok(rooms);
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao alterar senha: " + ex.Message);
        }
    }

    [Authorize(Roles = "Support")]
    public static IResult GetVerifyId(HttpContext http, int idSenac)
    {
        try
        {
            var context = new EngenhariasSenacContext();

            var dalStudent = new DAL<Student>(context);
            var student = dalStudent.SelectWhere(a => a.IdSenac == idSenac);

            if (student == null)
                return Results.NotFound("Estudante de id " + idSenac + " não encontrado");

            return Results.Ok(student.Fullname);
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao alterar senha: " + ex.Message);
        }
    }

    [Authorize(Roles = "Support")]
    public static IResult PostRegisterLog([FromBody] RegisterLogDto data, HttpContext http)
    {
        try
        {
            var context = new EngenhariasSenacContext();

            var dalStudent = new DAL<Student>(context);
            var student = dalStudent.SelectWhere(a => a.IdSenac == data.IdSenac);

            var dalLectures = new DAL<Lecture>(context);
            var lectures = dalLectures.SelectWhere(a => a.Room == data.Room);

            if (student == null)
                return Results.NotFound("Estudante não encontrado");

            if (lectures == null)
                return Results.NotFound("Sala inválida");

            // Obtém a hora atual de Brasília
            TimeZoneInfo brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            DateTime now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brasiliaTimeZone);

            var log = data.Type == "in" ? LogType.In : LogType.Out;
            var logType = data.Type == "in" ? "Entrada" : "Saída";

            var dalLectureLogs = new DAL<LectureLog>(context);

            // Busca o último log desse estudante, ordenado pela data mais recente
            var lastLog = dalLectureLogs
                .SelectWhereList(x => x.Student == student.CodStudents, q => q.OrderByDescending(x => x.Datetime))
                .FirstOrDefault();

            if (lastLog != null && lastLog.LogType == log)
            {
                return Results.Conflict("O último registro de " + student.Fullname + " foi " + logType);
            }

            var newLog = new LectureLog
            {
                Student = student.CodStudents,
                Datetime = now,
                Room = data.Room,
                LogType = log
            };

            dalLectureLogs.Insert(newLog);

            return Results.Ok("Registrado " + logType + " para " + student.Fullname);
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao alterar senha: " + ex.Message);
        }
    }

    [Authorize(Roles = "Support")]
    public static IResult GetAllPlatformIssues(HttpContext http, bool? onlyUnsolved)
    {
        try
        {
            var context = new EngenhariasSenacContext();

            var query = context.PlatformIssues
                .Join(context.Students,
                    issue => issue.StudentId,
                    student => student.CodStudents,
                    (issue, student) => new { issue, student });

            if (onlyUnsolved == true)
                query = query.Where(x => !x.issue.Checked);

            var result = query
                .OrderByDescending(x => x.issue.SentAt)
                .Select(x => new
                {
                    student = new
                    {
                        x.student.CodStudents,
                        x.student.IdSenac,
                        x.student.InstitutionalEmail,
                        x.student.PersonalEmail,
                        x.student.Cellphone,
                        x.student.Semester
                    },
                    issue = new
                    {
                        x.issue.CodIssue,
                        x.issue.Title,
                        x.issue.Description,
                        x.issue.SentAt,
                        x.issue.ImageUrl1,
                        x.issue.ImageUrl2,
                        x.issue.Checked,
                        x.issue.ResolutionComment
                    }
                })
                .ToList();

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao buscar solicitações: " + ex.Message);
        }
    }

    [Authorize(Roles = "Support")]
    public static IResult PatchCloseIssue(HttpContext http, int codIssue, [FromBody] CloseIssueDto data)
    {
        try
        {
            var context = new EngenhariasSenacContext();
            var dalIssue = new DAL<PlatformIssue>(context);

            var issue = dalIssue.SelectWhere(i => i.CodIssue == codIssue);
            if (issue == null)
                return Results.NotFound("Solicitação não encontrada");

            issue.Checked = true;
            issue.ResolutionComment = string.IsNullOrWhiteSpace(data.ResolutionComment) ? null : data.ResolutionComment;

            dalIssue.Update(issue);

            var dalStudent = new DAL<Student>(context);
            var student = dalStudent.SelectWhere(s => s.CodStudents == issue.StudentId);

            if (student != null)
            {
                var commentSection = string.IsNullOrWhiteSpace(issue.ResolutionComment)
                    ? "<p>Nenhum comentário adicional foi registrado.</p>"
                    : "<br><p><strong>A resolução do seu problema foi esta:</strong></p><blockquote style=\"border-left: 4px solid #ccc; margin: 0; padding: 8px 16px; color: #555;\">" + issue.ResolutionComment + "</blockquote><br>";

                var emailBody = "<h1>Sua solicitação foi resolvida!</h1>" +
                    "<p>Olá, <strong>" + student.Fullname + "</strong>!</p>" +
                    "<p>O chamado que você abriu na plataforma foi encerrado. Veja os detalhes abaixo:</p>" +
                    "<p><strong>Título:</strong> " + issue.Title + "</p>" +
                    "<p><strong>Descrição:</strong> " + issue.Description + "</p>" +
                    commentSection +
                    "<p>Qualquer dúvida estamos à disposição!</p>" +
                    "<p>Comitê das Engenharias Senac</p>";

                SendEmail.Send(
                    student.PersonalEmail ?? string.Empty,
                    student.InstitutionalEmail,
                    "Sua solicitação foi resolvida",
                    emailBody);
            }

            return Results.Ok("Solicitação encerrada com sucesso");
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao encerrar solicitação: " + ex.Message);
        }
    }
}
