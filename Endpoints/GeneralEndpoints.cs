using EngenhariasSenac.Controllers;

namespace EngenhariasSenac.Endpoints
{
    public static class GeneralEndpoints
    {
        public static void AddGeneralEndpoints(this WebApplication app)
        {
            app.MapGet("/api/Public/Projects", GeneralController.GetProjects);

            app.MapGet("/api/Public/company-assessments/{codCompany}", GeneralController.GetCompanyAssessments);

            app.MapGet("/api/Public/companies", GeneralController.GetAllCompanies);
        }
    }
}
