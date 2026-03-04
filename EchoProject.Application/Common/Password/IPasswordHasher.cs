namespace EchoProject.Application.Common.Password
{
    public interface IPasswordHasher
    {
        bool Validate(string inpPassword, string realPassword);
        string Hash(string password);
    }
}