using AutoMapper;
using EchoProject.Application.Common;
using EchoProject.Application.Common.PaginatedList;
using EchoProject.Application.DTO;
using EchoProject.Application.DTO.Projects;
using EchoProject.Application.Exceptions;
using EchoProject.Application.Requests.Projects;
using EchoProject.Domain.Interfaces;
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.ValueObjects;
using EchoProject.Domain.VendorAggregate;
using EchoProject.Infrastructure.Blockchain.Interfaces;
using Microsoft.Extensions.Logging;
namespace EchoProject.Application.Services
{
    [AppService]
    public class ProjectService(IUnitOfWork unitOfWork, IMapper mapper, IEthereumService ethereumService, ILogger<ProjectService> logger)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ProjectService> _logger = logger;
        private readonly IEthereumService _ethereumService = ethereumService;

        public PaginatedList<ProjectDTO> GetByNGO(Guid ngoId, int page, int pageSize)
        {
            var projects = _unitOfWork.Projects.FindByManager(ngoId);
            return projects.Select(x => _mapper.Map<ProjectDTO>(x)).Paginate(page, pageSize);
        }

        public async Task<ProjectDTO> CreateAsync(CreateProjectRequest projectRequest, UserDTO user)
        {
            var project = new Project(
                projectRequest.Title,
                projectRequest.Description,
                user.Id
            );

            await ProcessProjectGoalsAsync(project, projectRequest.Goals);

            await PrepareSmartContractAsync(project);
            
            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ProjectDTO>(project);
        }

        private async Task ProcessProjectGoalsAsync(Project project, IEnumerable<GoalRequest> goalRequests)
        {
            foreach (var goalReq in goalRequests)
            {
                var goalType = await _unitOfWork.GoalTypes.FindByIdAsync(goalReq.GoalTypeId)
                    ?? throw new NotFoundException($"GoalType {goalReq.GoalTypeId} not found.");

                var goal = project.AddGoal(goalReq.Title, goalReq.TargetAmount, goalType);
                if (goalReq.VendorIds != null && goalReq.VendorIds.Count > 0)
                {
                    var vendors = await FetchVendorsAsync(goalReq.VendorIds);
                    goal.AssignVendors(vendors);
                }
            }
        }
        private async Task<IEnumerable<Vendor>> FetchVendorsAsync(IEnumerable<Guid> vendorIds)
        {
            var vendors = new List<Vendor>();
            foreach (var id in vendorIds)
            {
                var vendor = await _unitOfWork.Vendors.FindByIdAsync(id)
                    ?? throw new NotFoundException($"Vendor {id} not found.");
                vendors.Add(vendor);
            }
            return vendors;
        }

        public async Task<ProjectDTO> UpdateAsync(Guid projectId, UpdateProjectRequest projectRequest, UserDTO user)
        {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId)
                ?? throw new NotFoundException($"Project with ID {projectId} not found.");

            if (project.ManagerId != user.Id)
                throw new UnauthorizedException("Only the project manager can update the project.");

            project.UpdateDetails(projectRequest.Title, projectRequest.Description);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ProjectDTO>(project);
        }

        private async Task<string> PrepareSmartContractAsync(Project project)
        {
            try
            {
                var smcAddress = await _ethereumService.DeployProjectContractAsync();
                project.SetSmartContractAddress(smcAddress);
                return smcAddress;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deploy smart contract for project {ProjectId}", project.Id);
                throw new ApplicationException("Failed to prepare blockchain smart contract for the project.", ex);
            }
        }
        public async Task<GoalDTO> AddGoalAsync(Guid projectId, GoalRequest goalRequest, UserDTO user)
        {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId)
                ?? throw new NotFoundException($"Project with ID {projectId} not found.");

            if (project.ManagerId != user.Id)
                throw new UnauthorizedException("Only the project manager can add goals to the project.");

            var goalType = await _unitOfWork.GoalTypes.FindByIdAsync(goalRequest.GoalTypeId)
                ?? throw new NotFoundException($"GoalType with ID {goalRequest.GoalTypeId} not found.");

            var goal = project.AddGoal(goalRequest.Title, goalRequest.TargetAmount, goalType);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<GoalDTO>(goal);
        }

        public async Task RemoveGoalAsync(Guid projectId, Guid goalId, UserDTO user)
        {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId)
                ?? throw new NotFoundException($"Project with ID {projectId} not found.");

            if (project.ManagerId != user.Id)
                throw new UnauthorizedException("Only the project manager can remove goals from the project.");

            var goal = project.RemoveGoal(goalId);
            _unitOfWork.Goals.Remove(goal);
            await _unitOfWork.CommitAsync();
        }

        public async Task<ProjectDTO> GetByIdAsync(Guid projectId)
        {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId)
                ?? throw new NotFoundException($"Project with ID {projectId} not found.");

            return _mapper.Map<ProjectDTO>(project);
        }
    }
}