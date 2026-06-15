using EngenhariasSenac.Banco;
using EngenhariasSenac.Database;
using EngenhariasSenac.Helpers;
using EngenhariasSenac.Models;
using EngenhariasSenac.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public static IResult GetAllCompanies()
    {
        try
        {
            var context = new EngenhariasSenacContext();

            var companies = context.Companies
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    codCompany = c.CodCompany,
                    name = c.Name,
                    description = c.Description,
                    picture = c.Picture
                })
                .ToList();

            return Results.Ok(companies);
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao buscar empresas: " + ex.Message);
        }
    }

    public static IResult GetCompanyAssessments(int codCompany)
    {
        try
        {
            var context = new EngenhariasSenacContext();

            var company = context.Companies.FirstOrDefault(c => c.CodCompany == codCompany);
            if (company == null)
                return Results.NotFound("Empresa não encontrada");

            var assessments = context.BusinessAssessment
                .Where(a => a.CompanyEvaluated == codCompany)
                .ToList();

            var studentIds = assessments.Select(a => a.StudentEvaluator).Distinct().ToList();

            var students = context.Students
                .Where(s => studentIds.Contains(s.CodStudents))
                .ToList();

            var result = assessments.Select(a =>
            {
                var student = students.FirstOrDefault(s => s.CodStudents == a.StudentEvaluator);
                return new
                {
                    assessment = a.Assessment,
                    picture = a.Picture,
                    comment = a.Comment,
                    evaluator = student == null ? null : (object)new
                    {
                        fullname = student.Fullname,
                        course = student.Course,
                        semester = student.Semester
                    }
                };
            }).ToList();

            return Results.Ok(new
            {
                companyName = company.Name,
                totalAssessments = assessments.Count,
                data = result
            });
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao buscar avaliações da empresa: " + ex.Message);
        }
    }

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
