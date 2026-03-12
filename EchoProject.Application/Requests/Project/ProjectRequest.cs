namespace EchoProject.Application.Requests.Project
{
    public record ProjectRequest
    (
        string Title, 
        string Description, 
        List<GoalRequest> Goals
    );
}