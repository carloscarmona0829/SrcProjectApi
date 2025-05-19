using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SrcProject.Configuration
{
    public class ApplicationDbContextFactory
: IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("cnn"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}

/*La clase ApplicationDbContextFactory solo se usa en tiempo de diseño, es decir, cuando ejecutas comandos como:

Add - Migration
Update - Database

Su único propósito es decirle a Entity Framework cómo crear una instancia de ApplicationDbContext 
sin necesidad de levantar toda la aplicación (por ejemplo, sin construir todos los servicios como UserManager, Jwt, etc.).*/