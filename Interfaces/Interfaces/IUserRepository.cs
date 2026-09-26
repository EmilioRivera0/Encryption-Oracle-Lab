using EncryptionOracleVulnerability.Entities;

namespace EncryptionOracleVulnerability.Interfaces.Interfaces
{
    public interface IUserRepository
    {
        UserEntity? GetUserById(string id);

        UserEntity? GetUser(string username, string password);

        IReadOnlyList<UserEntity> GetAllUsers();
    }
}
