namespace EchoProject.Application.Requests.Signup
{
    public record AddressRequest
    (
        string Street,
        string City, 
        string State, 
        string ZipCode, 
        int? Number, 
        string CountryCode, 
        string Neighborhood
    );
}