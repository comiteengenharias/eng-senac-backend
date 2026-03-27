using EngenhariasSenac.Controllers;

namespace EngenhariasSenac.Endpoints;

public static class StudentEndpoints
{    public static void AddStudentEndpoints(this WebApplication app)
    {
        app.MapGet("/api/student/info", StudentController.GetMyStudentData)
            .RequireAuthorization();

        app.MapPost("/api/student/change-password", StudentController.PostChangePassword)
            .RequireAuthorization();

        app.MapGet("/api/student/summary-info", StudentController.GetMySummaryData)
            .RequireAuthorization();

        app.MapGet("/api/student/project-info", StudentController.GetMyProjectData)
            .RequireAuthorization();

        app.MapGet("/api/student/lectures", StudentController.GetLecturesData)
            .RequireAuthorization();

        app.MapGet("/api/student/business", StudentController.GetBusinessData)
            .RequireAuthorization();

        app.MapPost("/api/student/business-assessment", (Delegate)StudentController.PostBusinessAssessment)
            .RequireAuthorization();

        app.MapPost("/api/student/other-projects", StudentController.GetOtherProjectsData)
            .RequireAuthorization();

        app.MapPost("/api/student/evaluate-other-projects", (Delegate)StudentController.PostEvaluateOtherProjects)
            .RequireAuthorization();

        app.MapPost("/api/student/deliver-project", (Delegate)StudentController.PostDeliverProject)
            .RequireAuthorization();

    }
}
