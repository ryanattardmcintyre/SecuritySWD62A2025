using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SecuritySWD62A2025.Models;
using SecuritySWD62A2025.Utilities;

namespace SecuritySWD62A2025.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index([FromServices] EncryptionUtility utility)
        {

            SymmetricKeys myKeys = new SymmetricKeys();
            myKeys.IV = new byte[] { 238, 31, 193, 59, 167, 91, 180, 116, 15, 237, 122, 149, 202, 128, 137, 22 };
            myKeys.SecretKey = new byte[] { 195, 189, 150, 23, 228, 157, 103, 100, 21, 59, 238, 164, 205, 225, 39, 66, 150, 88, 3, 111, 245, 211, 108, 154, 230, 164, 163, 159, 70, 20, 130, 72 };

            byte[] input = Encoding.UTF32.GetBytes("Rusty Rail");
            MemoryStream msin = new MemoryStream(input);

            var myAlg = System.Security.Cryptography.Aes.Create();
            myAlg.Mode = System.Security.Cryptography.CipherMode.ECB;
            myAlg.Padding = System.Security.Cryptography.PaddingMode.ANSIX923;


            MemoryStream msOut = utility.SymmetricEncrypt(msin, myAlg, myKeys);
            string base64String = Convert.ToBase64String(msOut.ToArray());





            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
