using EncryptionOracleVulnerability.Entities;
using EncryptionOracleVulnerability.Interfaces.Interfaces;
using EncryptionOracleVulnerability.Services.Interfaces;

namespace EncryptionOracleVulnerability.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;

        public LoginService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserEntity? LogIn(string username, string password)
        {
            return _userRepository.GetUser(username, password);
        }
    }
}
