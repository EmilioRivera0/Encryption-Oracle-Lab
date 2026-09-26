using EncryptionOracleVulnerability.Entities;

namespace EncryptionOracleVulnerability.Services.Interfaces
{
    public interface IUserService
    {
        UserEntity? GetUserInformation(string id);

        IReadOnlyList<UserEntity> GetAllUsers();
    }
}
