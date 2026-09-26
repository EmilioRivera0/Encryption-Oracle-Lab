namespace EncryptionOracleVulnerability.Entities
{
    public class BlobEntity
    {
        public BlobEntity(int id, string fileName, Guid blobGuid)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

            Id = id;
            FileName = fileName;
            BlobGuid = blobGuid;
        }

        public int Id { get; }

        public string FileName { get; }

        public Guid BlobGuid { get; }
    }
}
