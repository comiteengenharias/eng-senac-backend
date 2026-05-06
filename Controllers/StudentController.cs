using EngenhariasSenac.Banco;
using EngenhariasSenac.Database;
using EngenhariasSenac.Helpers;
using EngenhariasSenac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EngenhariasSenac.Dtos;
using EngenhariasSenac.Migrations;

namespace EngenhariasSenac.Controllers;

[Authorize(Roles = "Student")]
[ApiController]
[Route("api/student")]
public class StudentController : ControllerBase
{
    public static Student? GetAuthenticatedStudent(HttpContext http)
    {
        try
        {
            var codClaim = http.User.FindFirst("CodStudent")?.Value;
            if (string.IsNullOrEmpty(codClaim))
                return null;

            int codStudent = int.Parse(codClaim);

            var context = new EngenhariasSenacContext();
            var dal = new DAL<Student>(context);
            var student = dal.SelectWhere(s => s.CodStudents == codStudent);

            return student;
        }
        catch (Exception)
        {
            return null;
        }
    }

    [HttpGet("/api/student/info")]
    public static IResult GetMyStudentData(HttpContext http)
    {
        try
        {
            var student = GetAuthenticatedStudent(http);
            if (student == null)
                return Results.NotFound("Estudante não encontrado");

            var context = new EngenhariasSenacContext();

            // ============= Calcular Ponto Extra =============
            double extraNote = 0;
            string extraNoteReason = "";

            // Buscar IDs dos membros do comitê do banco de dados
            var committeeLeaders = context.Committee
                .Where(c => c.Role == "Leader")
                .Select(c => c.IdSenac)
                .ToList();
            
            var committeeParticipants = context.Committee
                .Where(c => c.Role == "Member")
                .Select(c => c.IdSenac)
                .ToList();

            int idSenac = student.IdSenac ?? 0;

            if (committeeLeaders.Contains(idSenac))
            {
                extraNote = 1.0;
                extraNoteReason = "Líder de Comitê";
            }
            else if (committeeParticipants.Contains(idSenac))
            {
                extraNote = 1.0;
                extraNoteReason = "Participante de Comitê";
            }
            else
            {
                // Calcular presença em palestras
                var allLectures = context.Lectures.ToList();
                var lectureLogs = context.LectureLogs
                    .Where(l => l.Student == student.CodStudents)
                    .ToList();

                int presentCount = 0;
                foreach (var lecture in allLectures)
                {
                    var logs = lectureLogs
                        .Where(l => l.Room == lecture.Room &&
                                    l.Datetime.Date == lecture.DatetimeStart.Date)
                        .OrderBy(l => l.Datetime)
                        .ToList();

                    var stack = new Stack<DateTime>();
                    double totalMinutesPresent = 0;

                    foreach (var log in logs)
                    {
                        if (log.LogType == LogType.In)
                        {
                            stack.Push(log.Datetime);
                        }
                        else if (log.LogType == LogType.Out && stack.Count > 0)
                        {
                            var inTime = stack.Pop();
                            var outTime = log.Datetime;

                            if (inTime < lecture.DatetimeStart) inTime = lecture.DatetimeStart;
                            if (outTime > lecture.DatetimeEnd) outTime = lecture.DatetimeEnd;

                            if (inTime < outTime)
                                totalMinutesPresent += (outTime - inTime).TotalMinutes;
                        }
                    }

                    double lectureMinutes = (lecture.DatetimeEnd - lecture.DatetimeStart).TotalMinutes;
                    if (lectureMinutes > 0 && (totalMinutesPresent / lectureMinutes) * 100 >= 75)
                    {
                        presentCount++;
                    }
                }

                var businessAssessments = context.BusinessAssessment
                    .Count(a => a.StudentEvaluator == student.CodStudents);

                var projectAssessments = context.ProjectAssessment
                    .Count(a => a.StudentEvaluator == student.CodStudents);

                if (presentCount >= 4 && businessAssessments >= 5 && projectAssessments >= 5)
                {
                    extraNote = 0.5;
                    extraNoteReason = "Objetivos atingidos!";
                }
                else
                {
                    // Verificar data limite para mensagem customizada
                    var dateLimitStr = "2026-06-14";
                    var dateLimit = DateTime.ParseExact(dateLimitStr, "yyyy-MM-dd", null);
                    var today = DateTime.Now;

                    if (today <= dateLimit)
                    {
                        extraNoteReason = "Complete os objetivos para conquistar o ponto extra";
                    }
                    else
                    {
                        extraNoteReason = "Período encerrado - inelegível para ponto extra";
                    }
                }
            }

            return Results.Ok(new
            {
                student,
                extraNote,
                extraNoteReason
            });
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao buscar dados do estudante: " + ex.Message);
        }
    }

    [HttpGet("/api/student/change-password")]
    public static IResult PostChangePassword([FromBody] ChangePasswordDto data, HttpContext http)
    {
        try
        {
            var student = GetAuthenticatedStudent(http);
            if (student == null)
                return Results.NotFound("Estudante não encontrado");

            var context = new EngenhariasSenacContext();
            var dalStudent = new DAL<Student>(context);

            // verifica a senha atual
            if (!PasswordHasher.VerifyPassword(data.CurrentPsw, student.Password))
                return Results.Conflict("Senha atual incorreta");

            // criptografa nova senha
            var hashedPsw = PasswordHasher.HashPassword(data.NewPsw);
            student.Password = hashedPsw;

            dalStudent.Update(student);

            return Results.Ok("Senha alterada com sucesso");
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao alterar senha: " + ex.Message);
        }
    }

    [HttpGet("/api/student/project-info")]
    public static IResult GetMySummaryData(HttpContext http)
    {
        try
        {
            var student = GetAuthenticatedStudent(http);
            if (student == null)
                return Results.NotFound("Estudante não encontrado");

            var context = new EngenhariasSenacContext();

            var dalProject = new DAL<ProjectTeam>(context);
            var project = dalProject.SelectWhere(a => a.CodTeam == student.ProjectTeam);
            if (project == null)
            {
                return Results.NotFound("Projeto não encontrado");
            }

            var dalStudent = new DAL<Student>(context);
            var membersCount = dalStudent
                .SelectWhereList(a => a.ProjectTeam == project.CodTeam)
                .Count;

            var dalProjectAssessments = new DAL<ProjectAssessment>(context);
            var evaluatedProjects = dalProjectAssessments
                .SelectWhereList(a => a.StudentEvaluator == student.CodStudents)
                .Count;
            var goalEvaluatedProjects = 5;


            var dalBusiness = new DAL<Companies>(context);
            var goalEvaluatedBusiness = 5;

            var dalBusinessAssessments = new DAL<BusinessAssessment>(context);
            var evaluatedBusiness = dalBusinessAssessments
                .SelectWhereList(a => a.StudentEvaluator == student.CodStudents)
                .Count();

            var projectInfo = new
            {
                student = new
                {
                    name = student.Fullname,
                    idSenac = student.IdSenac,
                },
                project = new
                {
                    name = project.GroupName,
                    token = project.Token,
                    semester = project.Semester,
                    membersCount
                },
                reviews = new
                {
                    evaluatedBusiness = evaluatedBusiness + "/" + goalEvaluatedBusiness,
                    evaluatedProjects = evaluatedProjects + "/" + goalEvaluatedProjects,
                }
            };

            return Results.Ok(projectInfo);
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao buscar dados do estudante: " + ex.Message);
        }
    }

    [HttpGet("/api/student/project-info")]
    public static IResult GetMyProjectData(HttpContext http)
    {
        try
        {
            var student = GetAuthenticatedStudent(http);
            if (student == null)
                return Results.NotFound("Estudante não encontrado");

            var context = new EngenhariasSenacContext();

            var dalProject = new DAL<ProjectTeam>(context);
            var project = dalProject.SelectWhere(a => a.CodTeam == student.ProjectTeam);
            if (project == null)
            {
                return Results.NotFound("Projeto não encontrado");
            }

            var dalStudent = new DAL<Student>(context);
            var students = dalStudent.SelectWhereList(a => a.ProjectTeam == project.CodTeam);

            var memberNames = students.Select(s => s.Fullname).ToList();
            var isLeader = project.Leader == student.CodStudents;

            // Calcular ratings e ranking do projeto
            var projectRating = ProjectRankingCalculator.CalculateProjectRating(
                project.CodTeam,
                project.Semester,
                context
            );

            var dalAttachment = new DAL<ProjectAttachment>(context);
            var hasSendAttachment = false;
            var attachment = dalAttachment.SelectWhere(a => a.ProjectTeam == project.CodTeam);
            if (attachment != null)
                hasSendAttachment = true;

            var projectInfo = new
            {
                project,
                members = memberNames,
                isLeader,
                ratings = projectRating,
                hasSendAttachment
            };

            return Results.Ok(projectInfo);
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao buscar dados do estudante: " + ex.Message);
        }
    }

    [HttpGet("/api/student/lectures")]
    public static IResult GetLecturesData(HttpContext http)
    {
        try
        {
            var student = GetAuthenticatedStudent(http);
            if (student == null)
                return Results.NotFound("Estudante não encontrado");

            var context = new EngenhariasSenacContext();
            var dalLectures = new DAL<Lecture>(context);
            var lectures = dalLectures.Select(q => q.OrderBy(l => l.DatetimeStart));

            var dalLectureLogs = new DAL<LectureLog>(context);
            var dalLectureSpeakers = new DAL<LectureSpeaker>(context);

            var studentPresenceList = new List<object>();

            foreach (var lecture in lectures)
            {
                var logs = dalLectureLogs.SelectWhereList(
                    l => l.Student == student.CodStudents &&
                         l.Room == lecture.Room &&
                         l.Datetime.Date == lecture.DatetimeStart.Date
                ).OrderBy(l => l.Datetime).ToList();

                var stack = new Stack<DateTime>();
                double totalMinutesPresent = 0;
                var intervals = new List<object>();

                foreach (var log in logs)
                {
                    if (log.LogType == LogType.In)
                    {
                        stack.Push(log.Datetime);
                    }
                    else if (log.LogType == LogType.Out && stack.Count > 0)
                    {
                        var inTime = stack.Pop();
                        var outTime = log.Datetime;

                        if (inTime < lecture.DatetimeStart) inTime = lecture.DatetimeStart;
                        if (outTime > lecture.DatetimeEnd) outTime = lecture.DatetimeEnd;

                        if (inTime < outTime)
                        {
                            var minutes = (outTime - inTime).TotalMinutes;
                            totalMinutesPresent += minutes;

                            intervals.Add(new
                            {
                                In = inTime,
                                Out = outTime,
                                DurationMinutes = Math.Round(minutes, 2)
                            });
                        }
                    }
                }

                var totalLectureMinutes = (lecture.DatetimeEnd - lecture.DatetimeStart).TotalMinutes;
                double percentage = totalLectureMinutes > 0
                    ? (totalMinutesPresent / totalLectureMinutes) * 100
                    : 0;

                var lectureSpeaker = dalLectureSpeakers.SelectWhere(a => a.CodSpeaker == lecture.Speaker);

                studentPresenceList.Add(new
                {
                    Lecture = lecture,
                    LectureSpeaker = lectureSpeaker,
                    Intervals = intervals,
                    TotalMinutesPresent = Math.Round(totalMinutesPresent, 2),
                    AttendancePercentage = Math.Round(percentage, 2)
                });
            }

            return Results.Ok(studentPresenceList);
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao buscar dados: " + ex.Message);
        }
    }

    [HttpGet("/api/student/business")]
    public static IResult GetBusinessData(HttpContext http)
    {
        try
        {
            var student = GetAuthenticatedStudent(http);
            if (student == null)
                return Results.NotFound("Estudante não encontrado");

            var context = new EngenhariasSenacContext();
            var dalCompanies = new DAL<Companies>(context);
            var companies = dalCompanies.Select();

            var dalBusinessAssessment = new DAL<BusinessAssessment>(context);

            var studentBusinessAssessments = new List<object>();

            foreach (var item in companies)
            {
                var canEvaluate = true;
                var itemAssessment = dalBusinessAssessment.SelectWhere(a => a.StudentEvaluator == student.CodStudents && a.CompanyEvaluated == item.CodCompany);

                studentBusinessAssessments.Add(new
                {
                    Company = item,
                    canEvaluate
                });
            }

            return Results.Ok(studentBusinessAssessments);
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao buscar dados do estudante: " + ex.Message);
        }
    }

    [Authorize(Roles = "Student")]
    public static async Task<IResult> PostBusinessAssessment(HttpContext http)
    {
        try
        {
            var student = GetAuthenticatedStudent(http);
            if (student == null)
                return Results.NotFound("Estudante não encontrado");

            var form = await http.Request.ReadFormAsync();

            // Monta o DTO manualmente
            var dto = new BusinessAssessmentDto
            {
                CompanyId = int.TryParse(form["companyId"], out var cId) ? cId : 0,
                Assessment = int.TryParse(form["assessment"], out var nota) ? nota : -1,
                Comment = form["comment"].ToString()
            };

            if (dto.CompanyId <= 0)
                return Results.BadRequest("Código da empresa inválido");

            if (dto.Assessment < 0 || dto.Assessment > 10)
                return Results.BadRequest("Nota deve estar entre 0 e 10");

            var file = form.Files.FirstOrDefault();
            if (file == null || file.Length == 0)
                return Results.BadRequest("Imagem obrigatória");

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                return Results.BadRequest("Formato de imagem inválido. Use JPG ou PNG.");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var folderPath = Path.Combine("wwwroot", "public", "Storage", "12st_week", "business_assessments");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fullPath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("Storage", "12st_week", "business_assessments", fileName).Replace("\\", "/");

            var context = new EngenhariasSenacContext();
            var dal = new DAL<BusinessAssessment>(context);

            var newAssessment = new BusinessAssessment
            {
                StudentEvaluator = student.CodStudents,
                CompanyEvaluated = dto.CompanyId,
                Assessment = dto.Assessment,
                Picture = relativePath,
                Comment = dto.Comment
            };

            dal.Insert(newAssessment);

            return Results.Ok("Avaliação registrada com sucesso");
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao registrar avaliação: " + ex.Message);
        }
    }

    [Authorize(Roles = "Student")]
    [HttpPost("/api/student/other-projects")]
    public static IResult GetOtherProjectsData([FromBody] ProjectsFilterDto? filterDto, HttpContext http)
    {
        try
        {
            var student = GetAuthenticatedStudent(http);
            if (student == null)
                return Results.NotFound("Estudante não encontrado");

            filterDto ??= new ProjectsFilterDto();

            var context = new EngenhariasSenacContext();
            var dalProject = new DAL<ProjectTeam>(context);
            var projects = dalProject.SelectWhereList(
                a => (!filterDto.Semester.HasValue || a.Semester == filterDto.Semester.Value) 
                    && a.CodTeam != student.ProjectTeam
                    && (string.IsNullOrEmpty(filterDto.GroupName) || a.GroupName.ToLower().Contains(filterDto.GroupName.ToLower())),
                q => q.OrderBy(p => p.Semester).ThenBy(p => p.GroupName)
            );

            // ============= OTIMIZAÇÃO: Carregar TODAS as avaliações do estudante de uma vez =============
            var projectIds = projects.Select(p => p.CodTeam).ToList();

            var allAssessments = context.ProjectAssessment
                .Where(a => a.StudentEvaluator == student.CodStudents && projectIds.Contains(a.EvaluatedProject))
                .Select(a => a.EvaluatedProject)
                .ToList();

            var allProjects = new List<object>();

            foreach (var item in projects)
            {
                // Verificar em memória (sem queries adicionais!)
                var canEvaluate = true;
                var alreadyEvaluated = allAssessments.Contains(item.CodTeam);

                allProjects.Add(new
                {
                    Project = item,
                    CanEvaluate = canEvaluate,
                    AlreadyEvaluated = alreadyEvaluated
                });
            }

            return Results.Ok(allProjects);
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao buscar dados do estudante: " + ex.Message);
        }
    }

    [Authorize(Roles = "Student")]
    public static async Task<IResult> PostEvaluateOtherProjects(HttpContext http)
    {
        try
        {
            var student = GetAuthenticatedStudent(http);
            if (student == null)
                return Results.NotFound("Estudante não encontrado");

            var form = await http.Request.ReadFormAsync();
            var context = new EngenhariasSenacContext();

            // Monta o DTO manualmente
            var dto = new ProjectsAssessmentDto
            {
                ProjectId = int.TryParse(form["projectId"], out var cId) ? cId : 0,
                Assessment = int.TryParse(form["assessment"], out var nota) ? nota : -1,
                Comment = form["comment"].ToString()
            };

            var dalProject = new DAL<ProjectTeam>(context);
            var project = dalProject.SelectWhere(a => a.CodTeam == dto.ProjectId);

            if (dto.ProjectId <= 0 || project == null)
                return Results.BadRequest("Código do projeto inválido");

            if (dto.Assessment < 0 || dto.Assessment > 10)
                return Results.BadRequest("Nota deve estar entre 0 e 10");

            var file = form.Files.FirstOrDefault();
            if (file == null || file.Length == 0)
                return Results.BadRequest("Imagem obrigatória");

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                return Results.BadRequest("Formato de imagem inválido. Use JPG ou PNG.");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var folderPath = Path.Combine("wwwroot", "public", "Storage", "12st_week", "projects_assessments");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fullPath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("Storage", "12st_week", "projects_assessments", fileName).Replace("\\", "/");

            var dalProjAssessment = new DAL<ProjectAssessment>(context);

            var newAssessment = (new ProjectAssessment
            {
                StudentEvaluator = student.CodStudents,
                EvaluatedProject = dto.ProjectId,
                Assessment = dto.Assessment,
                Picture = relativePath,
                Comment = dto.Comment
            });

            dalProjAssessment.Insert(newAssessment);

            return Results.Ok("Avaliação registrada com sucesso");
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao registrar avaliação: " + ex.Message);
        }
    }

    [Authorize(Roles = "Student")]
    public static async Task<IResult> PostDeliverProject(HttpContext http)
    {
        try
        {
            var student = GetAuthenticatedStudent(http);
            if (student == null)
                return Results.NotFound("Estudante não encontrado");

            var form = await http.Request.ReadFormAsync();
            var files = form.Files;

            if (files.Count != 6)
                return Results.BadRequest("Seis arquivos devem ser enviados (2 PDFs e 4 imagens).");

            var context = new EngenhariasSenacContext();
            var dalAttachment = new DAL<ProjectAttachment>(context);

            var groupName = $"group_{student.ProjectTeam}";
            var semester = $"{student.Semester}st";
            var savePath = Path.Combine("wwwroot", "public", "Storage", "12st_week", "projects", semester, groupName);

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            var savedFiles = new List<ProjectAttachment>();

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];

                if (file.Length > 0)
                {
                    string extension = Path.GetExtension(file.FileName).ToLower();
                    string newFileName;
                    string description;

                    switch (i)
                    {
                        case 0:
                            newFileName = $"artigo{extension}";
                            description = "Artigo/Caderno";
                            break;
                        case 1:
                            newFileName = $"banner{extension}";
                            description = "Banner";
                            break;
                        default:
                            newFileName = $"foto{i - 1}{extension}";
                            description = "Foto";
                            break;
                    }

                    var fullPath = Path.Combine(savePath, newFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var relativePath = Path.Combine("Storage", "12st_week", "projects", semester, groupName, newFileName).Replace("\\", "/");

                    var attachment = new ProjectAttachment
                    {
                        Path = relativePath,
                        Description = description,
                        ProjectTeam = student.ProjectTeam
                    };

                    dalAttachment.Insert(attachment);
                    savedFiles.Add(attachment);
                }
            }

            return Results.Ok(new { message = "Arquivos salvos com sucesso", saved = savedFiles });
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao salvar arquivos: " + ex.Message);
        }
    }


}
