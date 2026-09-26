using EncryptionOracleVulnerability.Entities;
using EncryptionOracleVulnerability.Models;
using EncryptionOracleVulnerability.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EncryptionOracleVulnerability.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = Roles.SysAdmin)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = Roles.SysAdmin)]
        public IActionResult Accounts()
        {
            var model = _userService.GetAllUsers()
                .Select(user => new UserAccountViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Claims = user.Claims
                        .Select(claim => new UserClaimViewModel { Type = claim.Type, Value = claim.Value })
                        .ToList()
                })
                .ToList();

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = Roles.SysAdmin)]
        public IActionResult Metrics()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = Roles.SysAdmin)]
        public IActionResult ActivityLogs()
        {
            return View();
        }
    }
}
