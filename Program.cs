using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using EngenhariasSenac.Endpoints;
using DotNetEnv;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.HttpOverrides;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// carrega o arquivo .env da raiz
Env.Load();
var port = int.Parse(Environment.GetEnvironmentVariable("PORT") ?? "8080");

//builder.WebHost.ConfigureKestrel(opts =>
//{
//    opts.ListenAnyIP(port, lo =>
//        lo.UseHttps(pfx, ""));
//});


var codeJwt = Environment.GetEnvironmentVariable("CODE_JWT") ?? throw new Exception("Código JWT não definido");
var key = Encoding.ASCII.GetBytes(codeJwt);

// Configura autenticação JWT com leitura do cookie
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true; // true em produção
        options.SaveToken = true;

        // Lê o token do cookie chamado "jwt"
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsWithCredentials", policy =>
        policy
            // aceita qualquer origem, mesmo com credenciais
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("robotica", new() { Title = "Robótica - API", Version = "v1.0.0", Description = "API de autenticação e gerenciamento para Robótica" });
    c.SwaggerDoc("semana", new() { Title = "Semana das Engenharias - API", Version = "v1.0.0", Description = "API de gerenciamento para a Semana das Engenharias" });
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (docName == "robotica")
            return apiDesc.RelativePath?.StartsWith("api/robotica") == true;
        return apiDesc.RelativePath?.StartsWith("api/robotica") != true;
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

// ignora ciclos de referência ao serializar JSON
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "OK",
        time = DateTimeOffset.UtcNow
    });
}).AllowAnonymous();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

app.UseCors("CorsWithCredentials");
app.UseStaticFiles();

// middleware de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// endpoints
app.AddRegistersEndpoints();
app.AddLoginEndpoints();
app.AddStudentEndpoints();
app.AddTeacherEndpoints();
app.AddSupportEndpoints();
app.AddGeneralEndpoints();
app.AddRoboticaLoginEndpoints();
app.AddRoboticaEndpoints();

// swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/robotica/swagger.json", "Robótica - API");
    c.SwaggerEndpoint("/swagger/semana/swagger.json", "Semana das Engenharias");
    c.RoutePrefix = "swagger";
});

app.Run();
