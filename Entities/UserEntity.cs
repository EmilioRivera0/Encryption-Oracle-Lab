using System.Security.Claims;
using System.Text.Json.Serialization;

namespace EncryptionOracleVulnerability.Entities
{
    public class UserEntity
    {
        public const int IdLength = 5;

        public string Id { get; }

        public string Name { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public List<Claim> Claims { get; } = [];

        public UserEntity(string id, string name, string email, string password)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            ArgumentException.ThrowIfNullOrWhiteSpace(password);

            if (id.Length != IdLength)
            {
                throw new ArgumentException($"User Id must be exactly {IdLength} characters.", nameof(id));
            }

            Id = id;
            Name = name;
            Email = email;
            Password = password;
        }
    }
}
