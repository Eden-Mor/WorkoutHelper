using Microsoft.EntityFrameworkCore;
using WorkoutHelper.Shared.Models;
using WorkoutHelperAPI.Helper;

namespace WorkoutHelperAPI.Data
{
    public class AppDBContext(IConfiguration configuration) : DbContext
    {
        protected readonly IConfiguration Configuration = configuration;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var useLocalDB = false;
            var host = OSHelper.IsLinux() 
                    ? Configuration.GetSection("Database")["DockerDBHost"]
                    : useLocalDB 
                        ? "localhost"
                        : Configuration.GetSection("Database")["DirectServerDBHost"];
            

            //Connect to postgres with connection string from app settings
            var connectionString = Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Connection String missing.");

            connectionString = connectionString.Replace("{HOST}", host)
                                               .Replace("{DB_USERNAME}", DockerSecretHelper.ReadSecret("postgres-u"))
                                               .Replace("{DB_PASSWORD}", DockerSecretHelper.ReadSecret("postgres-p"));

            optionsBuilder.UseNpgsql(connectionString, builder =>
            {
                builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var schemaName = Configuration.GetSection("Database")["SchemaName"];
            modelBuilder.HasDefaultSchema(schemaName);
        }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<DatabaseLoginData> Users { get; set; }
    }
}
