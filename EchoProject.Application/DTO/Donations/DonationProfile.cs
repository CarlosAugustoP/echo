using EchoProject.Domain.DonationAggregate;

namespace EchoProject.Application.DTO.Donations
{
    public class DonationProfile : AutoMapper.Profile
    {
        public DonationProfile()
        {
            CreateMap<Donation, DonationDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))  
                .ForMember(dest => dest.NameItem, opt => opt.MapFrom(src => src.Goal.GoalType.Name))
                .ForMember(dest => dest.GoalName, opt => opt.MapFrom(src => src.Goal.Title))
                .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Goal.Project.Title))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Goal.Project.Id))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.DonorId, opt => opt.MapFrom(src => src.DonorId))
                .ForMember(dest => dest.GoalId, opt => opt.MapFrom(src => src.GoalId))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.TotalCost, opt => opt.MapFrom(src => src.TotalCost))
                .ForMember(dest => dest.TransactionHash, opt => opt.MapFrom(src => src.TransactionHash))
                .ReverseMap();
        }
    }
}