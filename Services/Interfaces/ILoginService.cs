using EncryptionOracleVulnerability.Entities;

namespace EncryptionOracleVulnerability.Services.Interfaces
{
    public interface ILoginService
    {
        UserEntity? LogIn(string username, string password);
    }
}
