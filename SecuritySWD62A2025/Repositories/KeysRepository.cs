using SecuritySWD62A2025.Data;
using SecuritySWD62A2025.Models.DatabaseModels;

namespace SecuritySWD62A2025.Repositories
{
    public class KeysRepository:BaseRepository
    {

        //what did i achieve by inheriting the BaseRepository,
        //achievement: all repository class that i might or have, will all be using the same object in memory i.e. dbContext
        //benefit: when i start a transaction
        //     (where i want - i can start a transaction on ArticlesRepository, OR I can start in ArtifactRepository)
        //        so transaction will keep track of any operation happening on the database irrelevant from where its happening
        public KeysRepository(ApplicationDbContext dbContext) : base(dbContext) 
        { 
        
        }

        public void AddKeys(AsymmetricKeys keys)
        { 
            _dbContext.AsymmetricKeys.Add(keys);
            _dbContext.SaveChanges();
        }

        public AsymmetricKeys GetKeys(string username)
        { 
            return _dbContext.AsymmetricKeys.SingleOrDefault(x=> x.Username == username);
        }

    }
}
