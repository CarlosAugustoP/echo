namespace EchoProject.Application.Requests.Projects
{
    public record CreateBlogPostRequest
    (
        string? HeaderImageBase64, 
        string Title,
        string Content
    );
}