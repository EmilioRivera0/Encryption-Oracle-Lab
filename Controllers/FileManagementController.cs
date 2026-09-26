using System.Security.Cryptography;
using System.Text;
using EncryptionOracleVulnerability.Entities;
using EncryptionOracleVulnerability.Models;
using EncryptionOracleVulnerability.Services.Interfaces;
using EncryptionOracleVulnerability.Utilities.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EncryptionOracleVulnerability.Controllers
{
    public class FileManagementController : Controller
    {
        private readonly IFileOperationsService _fileOperationsService;
        private readonly IEncryptionUtility _encryptionUtility;
        private readonly IConfiguration _configuration;

        public FileManagementController(
            IFileOperationsService fileOperationsService,
            IEncryptionUtility encryptionUtility,
            IConfiguration configuration)
        {
            _fileOperationsService = fileOperationsService;
            _encryptionUtility = encryptionUtility;
            _configuration = configuration;
        }
        [HttpGet]
        [Authorize(Roles = Roles.Staff)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = Roles.Staff)]
        public IActionResult Upload()
        {
            return View(new FileUploadViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Staff)]
        public IActionResult Upload(FileUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string storedFileName;
            try
            {
                storedFileName = _fileOperationsService.StoreFile(model.FileName, model.Base64Content);
            }
            catch (FormatException)
            {
                ModelState.AddModelError(nameof(model.Base64Content), "The file content is not a valid Base64 string.");
                return View(model);
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError(nameof(model.FileName), "The file name is invalid.");
                return View(model);
            }

            string password = _configuration["TokenEncryption:Password"]
                ?? throw new InvalidOperationException("TokenEncryption:Password is not configured.");
            byte[] salt = Convert.FromBase64String(_configuration["TokenEncryption:Salt"]
                ?? throw new InvalidOperationException("TokenEncryption:Salt is not configured."));

            byte[] encryptedFileName = _encryptionUtility.EncryptDataAesGcm(Encoding.UTF8.GetBytes(storedFileName), password, salt);
            string fileId = Convert.ToBase64String(encryptedFileName);

            var result = new FileUploadViewModel
            {
                FileId = fileId,
                DownloadUrl = Url.Action(nameof(Download), "FileManagement", new { fileId }, Request.Scheme)
            };

            ModelState.Clear();
            return View(result);
        }

        [HttpGet]
        [Authorize(Roles = Roles.Staff)]
        public IActionResult Download(string? fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                return View(new FileDownloadViewModel());
            }

            return View(BuildDownloadResult(fileId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Download))]
        [Authorize(Roles = Roles.Staff)]
        public IActionResult DownloadPost(FileDownloadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(Download), model);
            }

            return View(nameof(Download), BuildDownloadResult(model.FileId));
        }

        [HttpGet]
        [Authorize(Roles = Roles.Staff)]
        public IActionResult DownloadFile(string fileId)
        {
            string? fileName = TryDecryptFileId(fileId);
            if (fileName is null)
            {
                return BadRequest();
            }

            byte[]? content = TryGetFile(fileName);
            if (content is null)
            {
                return NotFound();
            }

            return File(content, "application/octet-stream", fileName);
        }

        private FileDownloadViewModel BuildDownloadResult(string fileId)
        {
            var model = new FileDownloadViewModel { FileId = fileId };

            string? fileName = TryDecryptFileId(fileId);
            if (fileName is null)
            {
                model.Status = FileDownloadStatus.InvalidFileId;
                return model;
            }

            model.DecryptedFileName = fileName;

            if (TryGetFile(fileName) is null)
            {
                model.Status = FileDownloadStatus.NotFound;
                return model;
            }

            model.Status = FileDownloadStatus.Found;
            model.FileDownloadUrl = Url.Action(nameof(DownloadFile), "FileManagement", new { fileId });
            return model;
        }

        private string? TryDecryptFileId(string? fileId)
        {
            if (string.IsNullOrWhiteSpace(fileId))
            {
                return null;
            }

            try
            {
                byte[] encrypted = Convert.FromBase64String(fileId.Trim());
                byte[] decrypted = _encryptionUtility.DecryptDataAesGcm(encrypted, GetEncryptionPassword(), GetEncryptionSalt());
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (Exception ex) when (ex is FormatException or CryptographicException)
            {
                return null;
            }
        }

        private byte[]? TryGetFile(string fileName)
        {
            try
            {
                return _fileOperationsService.GetFile(fileName);
            }
            catch (Exception ex) when (ex is ArgumentException or FormatException)
            {
                return null;
            }
        }

        private string GetEncryptionPassword()
        {
            return _configuration["TokenEncryption:Password"]
                ?? throw new InvalidOperationException("TokenEncryption:Password is not configured.");
        }

        private byte[] GetEncryptionSalt()
        {
            return Convert.FromBase64String(_configuration["TokenEncryption:Salt"]
                ?? throw new InvalidOperationException("TokenEncryption:Salt is not configured."));
        }
    }
}
