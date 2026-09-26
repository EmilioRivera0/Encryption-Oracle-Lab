using EncryptionOracleVulnerability.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EncryptionOracleVulnerability.Controllers
{
	public class StaffController : Controller
	{
		[HttpGet]
		[Authorize(Roles = Roles.Staff)]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		[Authorize(Roles = Roles.Staff)]
		public IActionResult Sales()
		{
			return View();
		}

		[HttpGet]
		[Authorize(Roles = Roles.Staff)]
		public IActionResult Budgets()
		{
			return View();
		}
	}
}
