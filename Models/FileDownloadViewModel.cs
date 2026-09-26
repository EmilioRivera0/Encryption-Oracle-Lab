using System.ComponentModel.DataAnnotations;

namespace EncryptionOracleVulnerability.Models
{
    public enum FileDownloadStatus
    {
        None,
        Found,
        NotFound,
        InvalidFileId
    }

    public class FileDownloadViewModel
    {
        [Required(ErrorMessage = "Please enter a file identifier.")]
        [Display(Name = "File identifier")]
        public string FileId { get; set; } = string.Empty;

        public FileDownloadStatus Status { get; set; } = FileDownloadStatus.None;

        public string? DecryptedFileName { get; set; }

        public string? FileDownloadUrl { get; set; }
    }
}
