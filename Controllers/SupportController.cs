using EngenhariasSenac.Banco;
using EngenhariasSenac.Database;
using EngenhariasSenac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using static EngenhariasSenac.Endpoints.SupportEndpoints;

namespace EngenhariasSenac.Controllers;

public class SupportController : ControllerBase
{

    /// <summary>
    /// Retorna a lista de salas disponíveis nas palestras.
    /// </summary>
    /// <param name="http">HttpContext do requisitante.</param>
    /// <returns>Lista de nomes de salas.</returns>
    [Authorize(Roles = "Support")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
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

    /// <summary>
    /// Verifica se o `idSenac` informado pertence a um estudante e retorna o nome.
    /// </summary>
    /// <param name="http">HttpContext do requisitante.</param>
    /// <param name="idSenac">Identificador institucional do estudante.</param>
    /// <returns>Nome do estudante ou 404 se não encontrado.</returns>
    [Authorize(Roles = "Support")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Registra entrada/saída do estudante em uma sala de palestra.
    /// </summary>
    /// <param name="data">Objeto com `IdSenac`, `Room` e `Type` ('in' ou 'out').</param>
    /// <param name="http">HttpContext do requisitante.</param>
    /// <returns>Mensagem de sucesso ou conflito caso repetido.</returns>
    [Authorize(Roles = "Support")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
}
