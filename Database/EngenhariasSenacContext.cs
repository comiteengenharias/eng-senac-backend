using EngenhariasSenac.Models;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace EngenhariasSenac.Database
{
    public class EngenhariasSenacContext : DbContext
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<ProjectTeam> ProjectTeams { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<LectureSpeaker> LectureSpeakers { get; set; }
        public DbSet<LectureLog> LectureLogs { get; set; }
        public DbSet<Companies> Companies { get; set; }
        public DbSet<BusinessAssessment> BusinessAssessment { get; set; }
        public DbSet<ProjectAssessment> ProjectAssessment { get; set; }
        public DbSet<ProjectNote> ProjectNotes { get; set; }
        public DbSet<ProjectAttachment> ProjectAttachment { get; set; }
        public DbSet<Committee> Committee { get; set; }
        public DbSet<ExtraPoint> ExtraPoints { get; set; }
    public DbSet<PlatformIssue> PlatformIssues { get; set; }

        private readonly string? connectionString;

        public EngenhariasSenacContext()
        {
            // Carrega as variáveis do arquivo .env
            Env.Load();

            // Pega o valor da variável de ambiente
            connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("A variável CONNECTION_STRING não foi encontrada no arquivo .env");

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
