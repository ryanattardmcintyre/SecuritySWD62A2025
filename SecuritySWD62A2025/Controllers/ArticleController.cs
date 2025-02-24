using Microsoft.AspNetCore.Mvc;
using SecuritySWD62A2025.Models.DatabaseModels;
using SecuritySWD62A2025.Repositories;
using System.Net;

namespace SecuritySWD62A2025.Controllers
{
    public class ArticleController : Controller
    {
        ArticlesRepository _articlesRepository;
        ArtifactsRepository _artifactRepository;
        public ArticleController(ArticlesRepository articlesRepository, ArtifactsRepository artifactsRepository) { 
         _articlesRepository = articlesRepository;
            _artifactRepository = artifactsRepository;
        }


        public IActionResult Index()
        {
            return View();
        }


        [HttpGet] //this is going to run first to load a blank page with just the input controls
        public IActionResult Create()
        { return View();  }

        [HttpPost] //this is going to run second, to submit the data by the user
        //iwebhostenvironment is a built-in framework service which facilitates the reading of folder paths
        //one way of requesting the IWebHostEnvironment is to use method injection
        //result: when the app is running host will be auto -initialized for me
        public IActionResult Create(string title, string content, IFormFile file, [FromServices] IWebHostEnvironment host )
        {
            //to do
            //1. validate the file being uploaded
            //2. test the entries being saved in the database
            //3. what if one of the things fails? -introducing transactions!
            //4. validators



            //xss safe
            title = WebUtility.HtmlEncode(title);
            content = WebUtility.HtmlEncode(content);

            //creation of an article
            Article myArticle = new Article()
            {
                Title = title,
                Content = content,
                AuthorFK = User.Identity.Name, //this is how to get the name of the logged in user
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            //save the article to the database
            _articlesRepository.AddArticle(myArticle);

            //code that will upload the file
            string uniqueFilename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
            string absolutePath = host.ContentRootPath + "//Data//UserFiles//" + uniqueFilename;

            if (file != null)
            {

                //file validation!!!!
                if(System.IO.Path.GetExtension(file.FileName) != ".pdf")
                {
                    TempData["error"] = "File type not allowed";
                    return View();
                }

                using (var fs = System.IO.File.OpenWrite(uniqueFilename))
                {
                    file.CopyTo(fs);
                } //this is the line which is going to close the copied file leaving the data on the webserver
            }

            //save the file entry into the database
            Artifact myArtifact = new Artifact()
            {
                ArticleFK = myArticle.Id,
                Path = "//Data//UserFiles//" + uniqueFilename
            };
            _artifactRepository.AddArtifact(myArtifact);
            


            return View();



        }
    }
}
