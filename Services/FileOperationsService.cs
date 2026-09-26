using EncryptionOracleVulnerability.Interfaces.Interfaces;
using EncryptionOracleVulnerability.Services.Interfaces;

namespace EncryptionOracleVulnerability.Services
{
	public class FileOperationsService : IFileOperationsService
	{
		private readonly string _storagePath;
		private readonly IBlobRepository _blobRepository;

		public FileOperationsService(IConfiguration configuration, IHostEnvironment environment, IBlobRepository blobRepository)
		{
			_blobRepository = blobRepository;
			string configuredPath = configuration["FileStorage:Path"] ?? "App_Data/UploadedFiles";
			_storagePath = Path.IsPathRooted(configuredPath)
				? configuredPath
				: Path.Combine(environment.ContentRootPath, configuredPath);
		}

		public string StoreFile(string fileName, string base64Content)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
			ArgumentException.ThrowIfNullOrWhiteSpace(base64Content);

			// Validates that the input is a well-formed Base64 string.
			_ = Convert.FromBase64String(base64Content);

			Guid blobGuid = Guid.NewGuid();

			Directory.CreateDirectory(_storagePath);
			File.WriteAllText(Path.Combine(_storagePath, blobGuid.ToString()), base64Content);

			_blobRepository.SaveBlob(fileName, blobGuid);

			return fileName;
		}

		public byte[]? GetFile(string fileName)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

			var blob = _blobRepository.GetBlob(fileName);
			if (blob is null)
			{
				return null;
			}

			string filePath = Path.Combine(_storagePath, blob.BlobGuid.ToString());
			if (!File.Exists(filePath))
			{
				return null;
			}

			return Convert.FromBase64String(File.ReadAllText(filePath));
		}
	}
}
