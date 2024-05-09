using System.ComponentModel.DataAnnotations;
using WorkoutHelper.Shared.Enums;

namespace WorkoutHelper.Shared.Models
{
    public class LoginData
    {
        public string? Username { get; set; } = string.Empty;
        public string? PasswordHash { get; set; } = string.Empty;
    }

    public class DatabaseLoginData : LoginData
    {
        public DatabaseLoginData() { }

        public int? Id { get; set; }
        public string Salt { get; set; } = string.Empty;
        public RolesEnum Role { get; set; } = RolesEnum.User;
    }
}
