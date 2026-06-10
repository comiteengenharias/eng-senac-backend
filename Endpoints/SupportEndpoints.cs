using EngenhariasSenac.Controllers;

namespace EngenhariasSenac.Endpoints;

public static class SupportEndpoints
{
    public class RegisterLogDto
    {
        public string Room { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int IdSenac { get; set; }
    }

    public class CloseIssueDto
    {
        public string? ResolutionComment { get; set; }
    }


    public static void AddSupportEndpoints(this WebApplication app)
    {
        app.MapGet("/api/support/all-rooms", SupportController.GetAllRooms)
            .RequireAuthorization();

        app.MapGet("/api/support/verify-id/{idSenac}", SupportController.GetVerifyId)
            .RequireAuthorization();

        app.MapPost("/api/support/register-log", SupportController.PostRegisterLog)
            .RequireAuthorization();

        app.MapGet("/api/support/platform-issues", SupportController.GetAllPlatformIssues)
            .RequireAuthorization();

        app.MapPatch("/api/support/platform-issues/{codIssue}/close", SupportController.PatchCloseIssue)
            .RequireAuthorization();

    }
}
