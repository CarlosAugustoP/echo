using AutoMapper;
using EchoProject.Application.Common;
using EchoProject.Application.Common.PaginatedList;
using EchoProject.Application.DTO.Projects;
using EchoProject.Application.Exceptions;
using EchoProject.Application.Requests.GoalType;
using EchoProject.Domain.Interfaces;
using EchoProject.Domain.ProjectAggregate;

namespace EchoProject.Application.Services
{
    [AppService]
    public class GoalService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<GoalTypeDTO> CreateGoalType(GoalTypeRequest req)
        {
            var goalType = await _unitOfWork.Goals.FindGoalTypeByNameAsync(req.Name);
            
            if (goalType != null)
                throw new ConflictException("CONFLICT", $"Já existe um tipo de meta com o nome {req.Name}.");

            var goal = new GoalType(req.Name, req.Description);
            
            _unitOfWork.Goals.AddGoalType(goal);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<GoalTypeDTO>(goal);
        }

        public PaginatedList<GoalTypeDTO> GetGoalTypes(int page, int pageSize)
        {
            var goalTypes = _unitOfWork.Goals.FindAllGoalTypes();
            return goalTypes
                .Paginate(page, pageSize)
                .Select(x => _mapper.Map<GoalTypeDTO>(x));
        }

    }
}
