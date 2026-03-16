namespace EchoProject.Application.Requests.Pagination
{
    public record PageRequest(int PageNumber = 0, int PageSize = 20);
}