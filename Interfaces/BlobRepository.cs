using EncryptionOracleVulnerability.Entities;
using EncryptionOracleVulnerability.Interfaces.Interfaces;

namespace EncryptionOracleVulnerability.Interfaces
{
    public class BlobRepository : IBlobRepository
    {
        private readonly object _lock = new();
        private BlobEntity[] _blobs = [];

        public BlobEntity SaveBlob(string fileName, Guid blobGuid)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

            lock (_lock)
            {
                var blob = new BlobEntity(_blobs.Length + 1, fileName, blobGuid);
                BlobEntity[] blobs = new BlobEntity[_blobs.Length + 1];
                Array.Copy(_blobs, blobs, _blobs.Length);
                blobs[^1] = blob;
                _blobs = blobs;
                return blob;
            }
        }

        public BlobEntity? GetBlob(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            lock (_lock)
            {
                return Array.FindLast(_blobs, blob => string.Equals(blob.FileName, fileName, StringComparison.Ordinal));
            }
        }
    }
}
