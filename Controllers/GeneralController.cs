using EngenhariasSenac.Banco;
using EngenhariasSenac.Database;
using EngenhariasSenac.Helpers;
using EngenhariasSenac.Models;
using EngenhariasSenac.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
namespace EngenhariasSenac.Controllers;

public class GeneralController : ControllerBase
{
    private static string? GetImageExtension(int semester, int codTeam, int fotoNumber)
    {
        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var basePath = Path.Combine(webRootPath, "public", "Storage", "12st_week", "projects", $"{semester}st", $"group_{codTeam}");
        
        var extensions = new[] { ".jpg", ".jpeg", ".png" };
        
        foreach (var extension in extensions)
        {
            var filePath = Path.Combine(basePath, $"foto{fotoNumber}{extension}");
            if (System.IO.File.Exists(filePath))
            {
                return extension.TrimStart('.');
            }
        }
        
        // Se não encontrar nenhuma extensão, retorna null
        return null;
    }

    /// <summary>
    /// Retorna projetos publicados (usado no site/landing page da Semana das Engenharias).
    /// </summary>
    /// <param name="http">HttpContext do requisitante (não obrigatória autenticação).</param>
    /// <returns>Lista de projetos com título, texto, imagem e paths de artigo/banner.</returns>
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public static IResult GetProjects(HttpContext http)
    {
        try
        {
            var context = new EngenhariasSenacContext();
            var dalProject = new DAL<ProjectTeam>(context);
            var dalProjectAttachment = new DAL<ProjectAttachment>(context);
            
            var projects = dalProject.Select(
                q => q.OrderBy(p => p.Semester)
            );

            var projectsList = new List<object>();

            foreach (var project in projects)
            {
                var imageExtension = GetImageExtension(project.Semester, project.CodTeam, 1);
                
                var projSemester = project.Semester;

                if (imageExtension != null)
                {
                    var imagePath = $"public/Storage/12st_week/projects/{project.Semester}st/group_{project.CodTeam}/foto1.{imageExtension}";

                    var articlePath = $"public/Storage/12st_week/projects/{project.Semester}st/group_{project.CodTeam}/artigo.pdf";

                    var bannerPath = $"public/Storage/12st_week/projects/{project.Semester}st/group_{project.CodTeam}/banner.pdf";

                    projectsList.Add(new
                    {
                        title = project.GroupName,
                        text = project.Description,
                        img = imagePath,
                        semester = projSemester,
                        articlePath,
                        bannerPath
                    });
                }
            }

            return Results.Ok(projectsList);
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao buscar dados: " + ex.Message);
        }
    }

}
