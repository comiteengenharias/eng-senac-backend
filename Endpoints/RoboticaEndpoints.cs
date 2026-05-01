using EngenhariasSenac.Controllers;

namespace EngenhariasSenac.Endpoints;

public static class RoboticaEndpoints
{
    public static void AddRoboticaEndpoints(this WebApplication app)
    {
        app.MapGet("/api/robotica/me", RoboticaController.GetUserInfo)
            .RequireAuthorization();

        app.MapPut("/api/robotica/me", RoboticaController.UpdateUser)
            .RequireAuthorization();

        app.MapPut("/api/robotica/password", RoboticaController.ChangePassword)
            .RequireAuthorization();

        app.MapDelete("/api/robotica/me", RoboticaController.DeleteAccount)
            .RequireAuthorization();
    }
}
