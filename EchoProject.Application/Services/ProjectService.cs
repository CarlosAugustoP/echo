using AutoMapper;
using EchoProject.Application.Common;
using EchoProject.Application.Common.PaginatedList;
using EchoProject.Application.DTO;
using EchoProject.Application.DTO.Projects;
using EchoProject.Application.Exceptions;
using EchoProject.Application.Requests.Docs;
using EchoProject.Application.Requests.Pagination;
using EchoProject.Application.Requests.Projects;
using EchoProject.Domain.Common;
using EchoProject.Domain.Interfaces;
using EchoProject.Domain.ProjectAggregate;
using EchoProject.Domain.ValueObjects;
using EchoProject.Domain.VendorAggregate;
using EchoProject.Infrastructure.Blockchain.Interfaces;
using EchoProject.Infrastructure.Storage.Client;
using Microsoft.Extensions.Logging;
namespace EchoProject.Application.Services
{
    [AppService]
    public class ProjectService(IUnitOfWork unitOfWork, IMapper mapper, IEthereumService ethereumService, ILogger<ProjectService> logger, IStorageClient storage)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ProjectService> _logger = logger;
        private readonly IEthereumService _ethereumService = ethereumService;
        private readonly IStorageClient _storage = storage;

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
                var goalType = await _unitOfWork.Goals.FindGoalTypeByIdAsync(goalReq.GoalTypeId)
                    ?? throw new NotFoundException($"Tipo de meta com ID {goalReq.GoalTypeId} não encontrado.");

                var goal = project.AddGoal(goalReq.Title, goalReq.TargetAmount, goalType, goalReq.CostPerUnit, goalReq.Description);
                
                var vendors = await FetchVendorsAsync(goalReq.VendorIds);
                goal.AssignVendors(vendors);
                
            }
        }
        private async Task<IEnumerable<Vendor>> FetchVendorsAsync(IEnumerable<Guid>? vendorIds)
        {
            if (vendorIds == null || !vendorIds.Any())
                return [];
                
            var vendors = new List<Vendor>();
            foreach (var id in vendorIds)
            {
                var vendor = await _unitOfWork.Vendors.FindByIdAsync(id)
                    ?? throw new NotFoundException($"Fornecedor com ID {id} não encontrado.");
                vendors.Add(vendor);
            }
            return vendors;
        }

        public async Task<ProjectDTO> UpdateAsync(Guid projectId, UpdateProjectRequest projectRequest, UserDTO user)
        {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId)
                ?? throw new NotFoundException($"Projeto com ID {projectId} não encontrado.");

            if (project.ManagerId != user.Id)
                throw new UnauthorizedException("Apenas o gestor do projeto pode atualizar o projeto.");

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
                throw new ApplicationException("Falha ao preparar o contrato inteligente do projeto na blockchain.", ex);
            }
        }
        public async Task<GoalDTO> AddGoalAsync(Guid projectId, GoalRequest goalRequest, UserDTO user)
        {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId)
                ?? throw new NotFoundException($"Projeto com ID {projectId} não encontrado.");

            if (project.ManagerId != user.Id)
                throw new UnauthorizedException("Apenas o gestor do projeto pode adicionar metas ao projeto.");

            var goalType = await _unitOfWork.Goals.FindGoalTypeByIdAsync(goalRequest.GoalTypeId)
                ?? throw new NotFoundException($"Tipo de meta com ID {goalRequest.GoalTypeId} não encontrado.");

            var goal = project.AddGoal(goalRequest.Title, goalRequest.TargetAmount, goalType, goalRequest.CostPerUnit);
            await _unitOfWork.Goals.AddAsync(goal);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<GoalDTO>(goal);
        }

        public async Task RemoveGoalAsync(Guid projectId, Guid goalId, UserDTO user)
        {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId)
                ?? throw new NotFoundException($"Projeto com ID {projectId} não encontrado.");

            if (project.ManagerId != user.Id)
                throw new UnauthorizedException("Apenas o gestor do projeto pode remover metas do projeto.");

            var goal = project.RemoveGoal(goalId);
            _unitOfWork.Goals.Update(goal);
            await _unitOfWork.CommitAsync();
        }

        public async Task<ProjectDTO> GetByIdAsync(Guid projectId)
        {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId)
                ?? throw new NotFoundException($"Projeto com ID {projectId} não encontrado.");

            return _mapper.Map<ProjectDTO>(project);
        }

        public async Task<ProjectDTO> UpdateMainImageAsync(Guid projectId, string? mainImage, UserDTO user)
        {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId)
                ?? throw new NotFoundException($"Projeto com ID {projectId} não encontrado.");

            if (project.ManagerId != user.Id)
                throw new UnauthorizedException("Apenas o gestor do projeto pode atualizar a imagem principal do projeto.");

            if (mainImage is null)
            {
                project.RemoveMainImage();
            }
            else
            {
                var url = await _storage.UploadFileAsync($"project_{projectId}_mainimage_{Guid.NewGuid()}", mainImage.ToStream());
                project.AddOrUpdateMainImage(url);
            }
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ProjectDTO>(project);
        }

        public async Task<ProjectDTO> AddImageAsync(Guid projectId, string imgBase64, UserDTO user)
        {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId)
                ?? throw new NotFoundException($"Projeto com ID {projectId} não encontrado.");

            if (project.ManagerId != user.Id)
                throw new UnauthorizedException("Apenas o gestor do projeto pode adicionar imagens ao projeto.");

            var url = await _storage.UploadFileAsync($"project_{projectId}_image_{Guid.NewGuid()}", imgBase64.ToStream());
            project.AddImage(url);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ProjectDTO>(project);
        }

        public async Task<ProjectDTO> RemoveImageAsync(Guid projectId, string imageUrl, UserDTO user)
        {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId)
                ?? throw new NotFoundException($"Projeto com ID {projectId} não encontrado.");

            if (project.ManagerId != user.Id)
                throw new UnauthorizedException("Apenas o gestor do projeto pode remover imagens do projeto.");

            project.RemoveImage(imageUrl); 
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ProjectDTO>(project);
        }

        public async Task<ProjectBlogPostDTO> AddBlogPostAsync(Guid projectId, CreateBlogPostRequest request, UserDTO user)
        {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId)
                ?? throw new NotFoundException($"Projeto com ID {projectId} não encontrado.");

            if (project.ManagerId != user.Id)
                throw new UnauthorizedException("Apenas o gestor do projeto pode adicionar publicações ao projeto.");

            string? headerImageUrl = null;
            
            if (request.HeaderImageBase64 != null)
            {
                using var stream = request.HeaderImageBase64.ToStream();
                headerImageUrl = await _storage.UploadFileAsync($"project_{projectId}_blogpost_{Guid.NewGuid()}", stream);
            }
            
            var blogPost = new ProjectBlogPost
            (
                headerImageUrl is not null ? new ImageUrl(headerImageUrl) : null,
                request.Title, 
                request.Content,
                project
            );

            project.AddBlogPost(blogPost);

            _unitOfWork.Projects.AddBlogPost(blogPost);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ProjectBlogPostDTO>(blogPost);
        }

        public PaginatedList<ProjectBlogPostHeaderDTO> GetBlogPostsbyProject(Guid projectId, PageRequest pr)
        {
            var blogPosts = _unitOfWork.Projects.FindAllProjectBlogPosts().Where(x => x.ProjectId == projectId)
                .Paginate(pr.PageNumber, pr.PageSize)
                .Select(_mapper.Map<ProjectBlogPostHeaderDTO>);

            return blogPosts;
        }

        public async Task<ProjectBlogPostDTO> GetBlogPostByIdAsync(Guid blogPostId)
        {
            var blogPost = await _unitOfWork.Projects.FindProjectBlogPostByIdAsync(blogPostId)
                ?? throw new NotFoundException($"Publicação com ID {blogPostId} não encontrada.");

            return _mapper.Map<ProjectBlogPostDTO>(blogPost);
        }

        public async Task AddImageToProjectBlogPostAsync(Guid projectId, Guid blogPostId, DocumentRequest req, UserDTO user)
        {
            var project = await _unitOfWork.Projects.FindByIdAsync(projectId)
                ?? throw new NotFoundException($"Projeto com ID {projectId} não encontrado.");

            if (project.ManagerId != user.Id)
                throw new UnauthorizedException("Apenas o gestor do projeto pode adicionar imagens ao projeto.");

            var blogPost = await _unitOfWork.Projects.FindProjectBlogPostByIdAsync(blogPostId)
                ?? throw new NotFoundException($"Publicação com ID {blogPostId} não encontrada.");

            using var stream = req.Base64String.ToStream();
            var url = await _storage.UploadFileAsync($"project_{projectId}_blogpost_{blogPostId}_{Guid.NewGuid()}", stream);

            blogPost.AddImage(new ImageUrl(url));
            await _unitOfWork.CommitAsync();
        }
        public async Task<PaginatedList<ProjectHeaderDTO>> GetTrendingProjectsAsync(PageRequest pr)
        {
            var projects = _unitOfWork.Projects.FindTrendingProjects();
            return projects.Paginate(pr.PageNumber, pr.PageSize).Select(x => _mapper.Map<ProjectHeaderDTO>(x));
        }

        public PaginatedList<ProjectHeaderDTO> Search(PageRequest pr, string? search)
        {
            return _unitOfWork.Projects.Search(search)
                .Paginate(pr.PageNumber, pr.PageSize)
                .Select(_mapper.Map<ProjectHeaderDTO>);
        }

        public async Task<PaginatedList<ProjectHeaderDTO>> GetForYouAsync(UserDTO user, PageRequest pr)
        {
            var projects = await _unitOfWork.Projects.FindForYou(user.Id);
            return projects.Paginate(pr.PageNumber, pr.PageSize).Select(x => _mapper.Map<ProjectHeaderDTO>(x));
        }

        public async Task<PaginatedList<ProjectBlogPostHeaderDTO>> GetBlogPostsForYouAsync(UserDTO user, PageRequest pr)
        {
            return _unitOfWork.Projects.FindBlogPostByUserInvolvement(user.Id)
                .Paginate(pr.PageNumber, pr.PageSize)
                .Select(_mapper.Map<ProjectBlogPostHeaderDTO>);
        }

    }
}
