using SecuritySWD62A2025.Data;
using SecuritySWD62A2025.Models.DatabaseModels;

namespace SecuritySWD62A2025.Repositories
{
    public class ArtifactsRepository: BaseRepository
    {
       
        public ArtifactsRepository(ApplicationDbContext dbContext): base(dbContext) 
        {
           
        }


        public void AddArtifact(Artifact artifact)
        {
            _dbContext.Artifacts.Add(artifact);
            _dbContext.SaveChanges();
        }

        public IQueryable<Artifact> GetArtifacts(string articleId)
        {
            return _dbContext.Artifacts.Where(x=>x.ArticleFK.ToString() ==articleId);
        }

    }
}
