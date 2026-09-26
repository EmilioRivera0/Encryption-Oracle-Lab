using System.Security.Claims;
using EncryptionOracleVulnerability.Models;
using EncryptionOracleVulnerability.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EncryptionOracleVulnerability.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = string.IsNullOrEmpty(userId) ? null : _userService.GetUserInformation(userId);

            if (user is null)
            {
                return NotFound();
            }

            var model = new UserAccountViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Claims = user.Claims
                    .Select(claim => new UserClaimViewModel { Type = claim.Type, Value = claim.Value })
                    .ToList()
            };

            return View(model);
        }
    }
}
