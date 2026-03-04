namespace EchoProject.Application.Requests.Signup
{
    public record AddressRequest(string Street, string City, string State, string ZipCode, string Number, string CountryCode);
}