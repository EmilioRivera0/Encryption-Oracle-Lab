namespace EncryptionOracleVulnerability.Models
{
    public class UserAccountViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public List<UserClaimViewModel> Claims { get; set; } = [];
    }

    public class UserClaimViewModel
    {
        public string Type { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;
    }
}
