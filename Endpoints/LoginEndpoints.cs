using EngenhariasSenac.Controllers;

namespace EngenhariasSenac.Endpoints;
public static class LoginEndpoints
{
    public class StudentLoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class TeacherLoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class SupportLoginDto
    {
        public string Password { get; set; } = null!;
    }

    public static void AddLoginEndpoints(this WebApplication app)
    {

        app.MapPost("/api/login/student", LoginController.PostStudentLogin);

        app.MapPost("/api/login/teacher", LoginController.PostTeacherLogin);

        app.MapPost("/api/login/support", LoginController.PostSupportLogin);

        app.MapPost("/api/verify-login", LoginController.PostVerifyLogin);

        app.MapPost("/api/logout", LoginController.PostLogout);

        app.MapPost("/api/recover-password/student", LoginController.RecoverStudentPassword);

        app.MapPost("/api/recover-password/teacher", LoginController.RecoverTeacherPassword);
    }
}
