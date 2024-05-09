using System.IdentityModel.Tokens.Jwt;
using WorkoutHelper.Shared.Enums;

namespace WorkoutHelper.Services
{

    public static class SessionService
    {
        public static string Token = string.Empty;
        public static string GetClaimFromToken(string claimName)
        {
            try
            {
                if (string.IsNullOrEmpty(Token))
                    throw new ArgumentException("Token cannot be null or empty.", nameof(Token));

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(Token) as JwtSecurityToken ?? throw new ArgumentException("Invalid JWT token.");

                var claimVaue = jsonToken.Claims.FirstOrDefault(claim => claim.Type == claimName);

                if (claimVaue == null)
                    throw new ArgumentException($"{claimName} claim not found in token.");

                return claimVaue.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        public static RolesEnum Role => Enum.TryParse(GetClaimFromToken("role"), out RolesEnum role) ? role : RolesEnum.User;
    }
}
