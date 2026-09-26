using EncryptionOracleVulnerability.Entities;

namespace EncryptionOracleVulnerability.Interfaces.Interfaces
{
    public interface IBlobRepository
    {
        BlobEntity SaveBlob(string fileName, Guid blobGuid);

        BlobEntity? GetBlob(string fileName);
    }
}
