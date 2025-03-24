using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuritySWD62A2025.ActionFilters;
using SecuritySWD62A2025.Models;
using SecuritySWD62A2025.Models.DatabaseModels;
using SecuritySWD62A2025.Repositories;
using System.Net;

namespace SecuritySWD62A2025.Controllers
{
    [Authorize]
    //the effect of such attribute enforces any user getting into this controller be logged in
    //on top of the controller means that all the actions inside the controller are protected
    public class ArticleController : Controller
    {
        ArticlesRepository _articlesRepository;
        ArtifactsRepository _artifactRepository;
        public ArticleController(ArticlesRepository articlesRepository, ArtifactsRepository artifactsRepository) { 
         _articlesRepository = articlesRepository;
            _artifactRepository = artifactsRepository;
        }

        //1. write the repo methods
        //2. controller action
        //3. create the view
        public IActionResult Index()
        {
            var list = _articlesRepository.GetArticles();

            return View(list);
        }


        [HttpGet] //this is going to run first to load a blank page with just the input controls
        public IActionResult Create()
        { return View();  }

        [HttpPost] //this is going to run second, to submit the data by the user
        //iwebhostenvironment is a built-in framework service which facilitates the reading of folder paths
        //one way of requesting the IWebHostEnvironment is to use method injection
        //result: when the app is running host will be auto -initialized for me

        [ValidateAntiForgeryToken]
        public IActionResult Create(string title, string content, IFormFile file, [FromServices] IWebHostEnvironment host )
        {

            var transaction = _articlesRepository._dbContext.Database.BeginTransaction();
            try
            {
                //to do
                //1. validate the file being uploaded
                //2. test the entries being saved in the database
                //3. what if one of the things fails? -introducing transactions!
                //4. validators

                //1. ----------------------------- SECTION 1 - SAVING ARTICLE --------------------------------------

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
                _articlesRepository.AddArticle(myArticle); //<<<<<<<<<<<<<<<<<<<<< saving into db
 

                //2. ----------------------------- SECTION 2 - UPLOADING --------------------------------------

                //code that will upload the file
                string uniqueFilename = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                string absolutePath = host.ContentRootPath + "//Data//UserFiles//" + uniqueFilename;

                if (file != null) //file validation
                {
                    if (file.Length > (1024 * 1024 * 1024))
                    {
                        TempData["error"] = "File sizes accepted up to 1MB";
                        throw new Exception("File sizes accepted up to 1MB");
                    }

                    if (System.IO.Path.GetExtension(file.FileName) != ".jpg" &&
                       System.IO.Path.GetExtension(file.FileName) != ".pdf")
                    {
                        TempData["error"] = "File type not allowed";
                        throw new Exception("File type not allowed");
                    }


                    //check the file header to make sure that this is a jpg or pdf 100%
                    //255 216
                    byte[] whitelistForJPG = { 255, 216 };

                    MemoryStream msIn = new MemoryStream();
                    file.CopyTo(msIn);

                    byte[] inputFile = msIn.ToArray();

                    for (int b = 0; b < whitelistForJPG.Length; b++)
                    {
                        if (whitelistForJPG[b] != inputFile[b])
                        {
                            TempData["error"] = "File type is not accepted. Upload jpg files";
                            throw new Exception(TempData["error"].ToString());
                        }
                    }

                    //C:/..../.../..../uniquefilename
                    using (var fs = System.IO.File.OpenWrite(absolutePath))
                    {
                        fs.Position = 0;
                        file.CopyTo(fs);
                    } //this is the line which is going to close the copied file leaving the data on the webserver
                }

                //3. ----------------------------- SECTION 3 - SAVING ARTICLE INFO IN DB --------------------------------------
                //save the file entry into the database
                Artifact myArtifact = new Artifact()
                {
                    ArticleFK = myArticle.Id,
                    Path = "//Data//UserFiles//" + uniqueFilename
                };
                _artifactRepository.AddArtifact(myArtifact);

                TempData["message"] = "Artifact saved successfully";

                transaction.Commit(); //saves permanently into the db
            }
            catch (Exception ex)
            {
                transaction.Rollback(); //closes off transaction hence temporary save data is erased
                TempData["error"] = "Operation failed";
            }

            return View();

        }


        //choosing identification data types as GUIDs over INT makes it more secure and more difficult
        //for an attacker to guess some other resource behind the id
        [ArticleActionFilter]
        public IActionResult Details(string id)
        {
            var article = _articlesRepository.GetArticles().SingleOrDefault(x => x.Id.ToString() == id);
            
            if(article == null)
            {
                return RedirectToAction("Index");
            }

            var files = _artifactRepository.GetArtifacts(id);

            DetailsArticleViewModel myModel = new DetailsArticleViewModel();
            myModel.Article = article;
            myModel.Artifacts = files.ToList();

            return View(myModel);
            
        }

        public IActionResult Download(string id) {
            return View(); //returns a file 
        }
    }
}
