using AutoMapper;
using EchoProject.Application.Common;
using EchoProject.Application.Common.PaginatedList;
using EchoProject.Application.DTO.Projects;
using EchoProject.Application.Exceptions;
using EchoProject.Application.Requests.GoalType;
using EchoProject.Domain.Interfaces;

namespace EchoProject.Application.Services
{
    [AppService]
    public class GoalService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<GoalTypeDTO> CreateGoalType(GoalRequest req)
        {
            var goalType = await _unitOfWork.GoalTypes.FindAsync(x => x.Name == req.Name)
                ?? throw new ConflictException($"GoalType with name {req.Name} already exists.");
            return _mapper.Map<GoalTypeDTO>(goalType);
        }

        public async Task<PaginatedList<GoalTypeDTO>> GetGoalTypes(int page, int pageSize)
        {
            var goalTypes = _unitOfWork.GoalTypes.FindAll();
            return goalTypes.Paginate(page, pageSize).Select(x => _mapper.Map<GoalTypeDTO>(x));
        }

    }
}