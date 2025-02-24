using SecuritySWD62A2025.Data;
using SecuritySWD62A2025.Models.DatabaseModels;

namespace SecuritySWD62A2025.Repositories
{
    //Repository classes will contain code which only interacts with the database
    //LINQ code
    public class ArticlesRepository
    {
        ApplicationDbContext _dbContext;
        public ArticlesRepository(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
        } 

        public void AddArticle(Article article) { 
          _dbContext.Articles.Add(article);
            _dbContext.SaveChanges();
        }

        public IQueryable<Article> GetArticles(string author) {
            return GetArticles().Where(x => x.AuthorFK == author);
        }

        public IQueryable<Article> GetArticles()
        {
            return _dbContext.Articles;
        }

        public void UpdateArticle(Article article) {
            var articleToUpdate = GetArticles()
                .SingleOrDefault(x => x.Id == article.Id);

            if (articleToUpdate != null)
            {
                articleToUpdate.Title = article.Title;
                articleToUpdate.Content = article.Content;
                articleToUpdate.UpdatedDate = DateTime.Now;

                _dbContext.SaveChanges();
            }
         }

        public void DeleteArticle(Guid id) {
            var articleToUpdate = GetArticles()
             .SingleOrDefault(x => x.Id == id);

            if (articleToUpdate != null)
            {
                _dbContext.Remove(articleToUpdate);
                _dbContext.SaveChanges();
            }

        }
    }
}
