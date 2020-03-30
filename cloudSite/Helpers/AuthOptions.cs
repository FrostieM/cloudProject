using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace cloudSite.Helpers
{
    public static class AuthOptions
    {
        public const string Issuer = "MyAuthServer";
        public const string Audience = "https://localhost/";
        private const string Key = "mysupersecret_secretkey!123"; 
        public const int Lifetime = 360; //minutes
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}