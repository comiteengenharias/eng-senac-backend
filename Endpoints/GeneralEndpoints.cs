using EngenhariasSenac.Controllers;

namespace EngenhariasSenac.Endpoints
{
    public static class GeneralEndpoints
    {
        public static void AddGeneralEndpoints(this WebApplication app)
        {
            app.MapGet("/api/Public/Projects", GeneralController.GetProjects);
        }
    }
}
