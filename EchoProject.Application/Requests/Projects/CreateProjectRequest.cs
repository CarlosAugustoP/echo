namespace EchoProject.Application.Requests.Projects
{
    public record CreateProjectRequest
    (
        string Title, 
        string Description, 
        List<GoalRequest> Goals
    );
}