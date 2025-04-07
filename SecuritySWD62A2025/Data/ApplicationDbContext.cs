using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecuritySWD62A2025.Models.DatabaseModels;


//Areas: you'll find built-in classes that will help you in managing user accounts
//       note: in Areas everthing is hidden unless you decide to customize any process
namespace SecuritySWD62A2025.Data
{
    //this is a representation (abstraction) of the database
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Artifact> Artifacts { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<AsymmetricKeys> AsymmetricKeys { get; set; }

    }
}
