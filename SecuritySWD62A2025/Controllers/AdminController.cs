using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecuritySWD62A2025.Models;
using SecuritySWD62A2025.Models.DatabaseModels;
using SecuritySWD62A2025.Repositories;

namespace SecuritySWD62A2025.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private ArticlesRepository _articlesRepository;
        private UserManager<IdentityUser> _userManager;

        //UserManager will allows us to query the list of users since that is being automatically managed
        //in AspNetUsers
        public AdminController(ArticlesRepository articlesRepository, UserManager<IdentityUser> userManager) {
          _articlesRepository = articlesRepository; 
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            //the Select (linq-to-entities) method will filter what to fetch from the db
            //its imp because instead of fetching everything from the db, we fetch only what we need
            //Select Email, Id, Address, PasswordHash, MobileNo, .... from AspNetUsers <<< insecure
            //Select Email from AspNetUsers

            var usersList = _userManager.Users.Select(x=>x.Email).ToList();
            var articlesList = from a in _articlesRepository.GetArticles()
                               select new Article() { Id = a.Id, Title = a.Title };
            // .Select(a => new Article { Id = a.Id, Title = a.Title });

            PermissionsViewModel model = new PermissionsViewModel()
            {
                Users = usersList!,
                Articles = articlesList.ToList()
            };

            return View(model);
        }

        public IActionResult AllocatePermissionToArticle(string email, string articleId)
        { 
            _articlesRepository.AddPermission(new Guid(articleId), email);
            TempData["success"] = "Article allocated to user with email " + email;
            return RedirectToAction("Index");
        }


        //exercise 1:
        //public IActionResult DeallocatePermissionFromArticle(string email, string articleId)
        //{
        //    //DeletePermission instead
        //}

        //exercise 2:
        //try to replicate the entire Permission allocation feature, but for users and Roles






    }
}
