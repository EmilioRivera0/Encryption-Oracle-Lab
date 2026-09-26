using EncryptionOracleVulnerability.Entities;
using EncryptionOracleVulnerability.Interfaces.Interfaces;
using EncryptionOracleVulnerability.Services.Interfaces;

namespace EncryptionOracleVulnerability.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserEntity? GetUserInformation(string id)
        {
            return _userRepository.GetUserById(id);
        }

        public IReadOnlyList<UserEntity> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }
    }
}
