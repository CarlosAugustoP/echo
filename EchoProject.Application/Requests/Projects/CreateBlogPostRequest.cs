namespace EchoProject.Application.Requests.Projects
{
    public record CreateBlogPostRequest
    (
        string? HeaderImageBase64, 
        string Content,
        List<string>? ImageBase64List, 
        Guid ProjectId
    );
}