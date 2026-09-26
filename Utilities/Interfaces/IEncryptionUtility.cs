namespace EncryptionOracleVulnerability.Utilities.Interfaces
{
    public interface IEncryptionUtility
    {
        byte[] EncryptDataAesGcm(byte[] data, string password, byte[] salt);

        byte[] DecryptDataAesGcm(byte[] data, string password, byte[] salt);
    }
}
