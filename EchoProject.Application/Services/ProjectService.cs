using AutoMapper;
using EchoProject.Application.Common;
using EchoProject.Application.Common.PaginatedList;
using EchoProject.Application.DTO;
using EchoProject.Application.DTO.Projects;
using EchoProject.Application.Exception;
using EchoProject.Application.Requests.Projects;
using EchoProject.Domain.Interfaces;
using EchoProject.Domain.ProjectAggregate;
namespace EchoProject.Application.Services
{
    [AppService]
    public class ProjectService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public PaginatedList<ProjectDTO> GetByNGO(Guid ngoId, int page, int pageSize)
        {
            var projects = _unitOfWork.Projects.FindByManager(ngoId);
            return projects.Select(x => _mapper.Map<ProjectDTO>(x)).Paginate(page, pageSize);
        }

       public async Task<ProjectDTO> CreateAsync(ProjectRequest projectRequest, UserDTO user)
       {
            var project = new Project(projectRequest.Title, projectRequest.Description, user.Id);

            foreach (var projectGoal in projectRequest.Goals)
            {
                var goalType = await _unitOfWork.GoalTypes.FindByIdAsync(projectGoal.GoalTypeId)
                    ?? throw new NotFoundException($"GoalType with ID {projectGoal.GoalTypeId} not found.");
                
                project.AddGoal(projectGoal.Title, projectGoal.TargetAmount, goalType);
            }

            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ProjectDTO>(project);
       }

       public async Task<ProjectDTO> GetByIdAsync(Guid projectId)
       {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId) 
                ?? throw new NotFoundException($"Project with ID {projectId} not found.");
            
            return _mapper.Map<ProjectDTO>(project);
       }
    }
}