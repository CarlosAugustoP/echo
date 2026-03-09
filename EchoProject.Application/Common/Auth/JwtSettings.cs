namespace EchoProject.Application.Common.Auth
{
     public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Expiration { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
    }
}