using System.Security.Claims;
using EncryptionOracleVulnerability.Entities;
using EncryptionOracleVulnerability.Interfaces.Interfaces;

namespace EncryptionOracleVulnerability.Interfaces
{
	public class UserRepository : IUserRepository
	{
		private static readonly UserEntity[] Users = CreateUsers();

		public UserEntity? GetUserById(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				return null;
			}

			return Users.FirstOrDefault(user => string.Equals(user.Id, id, StringComparison.OrdinalIgnoreCase));
		}

		public UserEntity? GetUser(string username, string password)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
			{
				return null;
			}

			return Users.FirstOrDefault(user =>
				string.Equals(user.Email, username, StringComparison.OrdinalIgnoreCase) &&
				string.Equals(user.Password, password, StringComparison.Ordinal));
		}

		public IReadOnlyList<UserEntity> GetAllUsers()
		{
			return Users;
		}

		private static UserEntity[] CreateUsers()
		{
			var sysAdmin = new UserEntity("A0001", "System Admin", "sysadmin@harvesttrustee.com", "Admin#2026");
			sysAdmin.Claims.Add(new Claim(ClaimTypes.Role, "SysAdmin"));

			return
			[
				sysAdmin,
				CreateStaffUser("U0001", "Alice Johnson", "alice.johnson@harvesttrustee.com", "Alice#2026"),
				CreateStaffUser("U0002", "Bob Smith", "bob.smith@harvesttrustee.com", "Bob#2026"),
				CreateStaffUser("U0003", "Carol Williams", "carol.williams@harvesttrustee.com", "Carol#2026"),
				CreateStaffUser("U0004", "David Brown", "david.brown@harvesttrustee.com", "David#2026"),
				CreateStaffUser("U0005", "Emma Davis", "emma.davis@harvesttrustee.com", "Emma#2026"),
				CreateStaffUser("U0006", "Frank Miller", "frank.miller@harvesttrustee.com", "Frank#2026"),
				CreateStaffUser("U0007", "Grace Wilson", "grace.wilson@harvesttrustee.com", "Grace#2026"),
				CreateStaffUser("U0008", "Henry Moore", "henry.moore@harvesttrustee.com", "Henry#2026")
			];
		}

		private static UserEntity CreateStaffUser(string id, string name, string email, string password)
		{
			var user = new UserEntity(id, name, email, password);
			user.Claims.Add(new Claim(ClaimTypes.Role, "Staff"));
			return user;
		}
	}
}
