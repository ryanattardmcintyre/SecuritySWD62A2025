using SecuritySWD62A2025.Data;
using SecuritySWD62A2025.Models.DatabaseModels;

namespace SecuritySWD62A2025.Repositories
{
    public class ArtifactsRepository
    {
        ApplicationDbContext _dbContext;
        public ArtifactsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public void AddArtifact(Artifact artifact)
        {
            _dbContext.Artifacts.Add(artifact);
            _dbContext.SaveChanges();
        }

    }
}
