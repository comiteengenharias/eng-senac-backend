using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using EngenhariasSenac.Endpoints;
using DotNetEnv;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.HttpOverrides;

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

var allowedOrigins = new[]
{
    "http://localhost:3000",
    "http://127.0.0.1:3000",
    "https://engenhariasenac.com.br",
    "https://www.engenhariasenac.com.br"
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsWithCredentials", policy =>
        policy
            .WithOrigins(allowedOrigins)
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
            .AllowAnyHeader()  
            .AllowCredentials()
    );
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseRouting();
app.UseCors("CorsWithCredentials");
app.UseStaticFiles();

// middleware de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

// endpoints
app.AddRegistersEndpoints();
app.AddLoginEndpoints();
app.AddStudentEndpoints();
app.AddTeacherEndpoints();
app.AddSupportEndpoints();
app.AddGeneralEndpoints();

// swagger
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
