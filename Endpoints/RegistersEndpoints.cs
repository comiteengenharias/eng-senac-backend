using EngenhariasSenac.Models;
using EngenhariasSenac.Controllers;

namespace EngenhariasSenac.Endpoints
{
    public static class RegistersEndpoints
    {
        public class LeaderRegistrationDto
        {
            public Student NewStudent { get; set; } = null!;
            public ProjectTeam NewProject { get; set; } = null!;
        }

        public class MemberRegistrationDto
        {
            public Student NewStudent { get; set; } = null!;
            public string Token { get; set; } = null!;
        }

        public class TeacherRegistrationDto
        {
            public Teacher NewTeacher { get; set; } = null!;
            public string Token { get; set; } = null!;
        }

        public static void AddRegistersEndpoints(this WebApplication app)
        {
            app.MapPost("/api/register/leader", RegistersController.PostLeaderRegistration);

            app.MapPost("/api/register/member", RegistersController.PostMemberRegistration);

            app.MapPost("/api/register/teacher", RegistersController.PostTeacherRegistration);
        }
    }
}
