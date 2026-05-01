using EngenhariasSenac.Controllers;

namespace EngenhariasSenac.Endpoints;

public static class RoboticaLoginEndpoints
{
    public static void AddRoboticaLoginEndpoints(this WebApplication app)
    {
        app.MapPost("/api/robotica/login", RoboticaLoginController.PostLogin)
            .AllowAnonymous();
        
        app.MapPost("/api/robotica/verify", RoboticaLoginController.PostVerify)
            .AllowAnonymous();

        app.MapPost("/api/robotica/register", RoboticaLoginController.PostRegister)
            .AllowAnonymous();
    }
}
