namespace EchoProject.Application.Requests.Projects
{
    public record ProjectRequest
    (
        string Title, 
        string Description, 
        List<GoalRequest> Goals
    );
}