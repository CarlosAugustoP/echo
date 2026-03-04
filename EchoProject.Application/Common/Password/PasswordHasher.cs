

using Bc = BCrypt.Net.BCrypt;
namespace EchoProject.Application.Common.Password
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return Bc.HashPassword(password);
        }

        public bool Validate(string inpPassword, string realPassword)
        {
            return Bc.Verify(inpPassword, realPassword);
        }

    }
}