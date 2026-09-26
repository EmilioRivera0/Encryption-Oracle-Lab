using Microsoft.AspNetCore.Mvc;

namespace EncryptionOracleVulnerability.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
