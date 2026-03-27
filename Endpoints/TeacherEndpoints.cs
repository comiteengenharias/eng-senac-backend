using EngenhariasSenac.Controllers;

namespace EngenhariasSenac.Endpoints;

public static class TeacherEndpoints
{
    public static void AddTeacherEndpoints(this WebApplication app)
    {
        app.MapGet("/api/Teacher/Info", TeacherController.GetMyTeacherData)
            .RequireAuthorization();

        app.MapPost("/api/Teacher/change-password", TeacherController.PostChangePassword)
            .RequireAuthorization();

        app.MapPost("/api/teacher/projects", TeacherController.GetProjectsData)
            .RequireAuthorization();

        app.MapPost("/api/teacher/evaluate-projects", TeacherController.PostEvaluateProjects)
            .RequireAuthorization();

        app.MapPost("/api/teacher/final-notes", TeacherController.GetFinalNotes)
            .RequireAuthorization();

        app.MapPost("/api/teacher/top-projects-ranking", TeacherController.GetTopProjectsBySemester)
            .RequireAuthorization();

        app.MapGet("/api/teacher/point-materials", TeacherController.GetPointMaterials)
            .RequireAuthorization();

    }
}
