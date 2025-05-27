using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SrcProject.Models.InModels.Security;

namespace SrcProject.Configuration
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUserIM>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}