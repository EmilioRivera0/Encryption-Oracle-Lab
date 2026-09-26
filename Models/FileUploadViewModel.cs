using System.ComponentModel.DataAnnotations;

namespace EncryptionOracleVulnerability.Models
{
    public class FileUploadViewModel
    {
        [Required(ErrorMessage = "Please select a file to upload.")]
        public string FileName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a file to upload.")]
        public string Base64Content { get; set; } = string.Empty;

        public string? FileId { get; set; }

        public string? DownloadUrl { get; set; }
    }
}
