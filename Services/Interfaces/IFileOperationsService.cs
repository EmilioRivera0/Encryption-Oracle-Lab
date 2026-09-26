namespace EncryptionOracleVulnerability.Services.Interfaces
{
    public interface IFileOperationsService
    {
        string StoreFile(string fileName, string base64Content);

        byte[]? GetFile(string fileName);
    }
}
