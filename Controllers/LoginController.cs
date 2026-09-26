using System.Text;
using System.Text.Json;
using EncryptionOracleVulnerability.Models;
using EncryptionOracleVulnerability.Services.Interfaces;
using EncryptionOracleVulnerability.Utilities.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EncryptionOracleVulnerability.Controllers
{
    public class LoginController : Controller
    {
        public const string AuthTokenCookieName = "AuthToken";

        private readonly ILoginService _loginService;
        private readonly IEncryptionUtility _encryptionUtility;
        private readonly IConfiguration _configuration;

        public LoginController(ILoginService loginService, IEncryptionUtility encryptionUtility, IConfiguration configuration)
        {
            _loginService = loginService;
            _encryptionUtility = encryptionUtility;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogIn(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Index), model);
            }

            var user = _loginService.LogIn(model.Username, model.Password);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login credentials.");
                model.Password = string.Empty;
                return View(nameof(Index), model);
            }

            string userJson = JsonSerializer.Serialize(user);
            string password = _configuration["TokenEncryption:Password"]
                ?? throw new InvalidOperationException("TokenEncryption:Password is not configured.");
            byte[] salt = Convert.FromBase64String(_configuration["TokenEncryption:Salt"]
                ?? throw new InvalidOperationException("TokenEncryption:Salt is not configured."));

            byte[] encryptedToken = _encryptionUtility.EncryptDataAesGcm(Encoding.UTF8.GetBytes(userJson), password, salt);

            Response.Cookies.Append(AuthTokenCookieName, Convert.ToBase64String(encryptedToken), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                IsEssential = true
            });

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOut()
        {
            Response.Cookies.Delete(AuthTokenCookieName, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return RedirectToAction("Index", "Home");
        }
    }
}
