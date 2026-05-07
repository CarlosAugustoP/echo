using AutoMapper;
using EchoProject.Application.Common;
using EchoProject.Application.Common.PaginatedList;
using EchoProject.Application.DTO;
using EchoProject.Application.DTO.Notifications;
using EchoProject.Application.Requests.Notifications;
using EchoProject.Application.Requests.Pagination;
using EchoProject.Domain.Interfaces;

namespace EchoProject.Application.Services
{
    [AppService]
    public class NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public PaginatedList<NotificationDTO> GetByUser(UserDTO user, PageRequest pageRequest)
        {
            return _unitOfWork.Notifications
                .FindByUserId(user.Id)
                .OrderByDescending(x => x.CreatedAt)
                .Paginate(pageRequest.PageNumber, pageRequest.PageSize)
                .Select(x => _mapper.Map<NotificationDTO>(x));
        }

        public async Task<UnreadNotificationsCountDTO> GetUnreadCountAsync(UserDTO user)
        {
            var count = await _unitOfWork.Notifications.CountUnreadByUserIdAsync(user.Id);
            return new UnreadNotificationsCountDTO { Count = count };
        }

        public async Task<MarkNotificationsAsReadResultDTO> MarkAsReadAsync(UserDTO user, MarkNotificationsAsReadRequest request)
        {
            if (request.NotificationIds.Count == 0)
                return new MarkNotificationsAsReadResultDTO { UpdatedCount = 0 };

            var notifications = await _unitOfWork.Notifications.FindByIdsAndUserIdAsync(request.NotificationIds, user.Id);

            var updatedCount = 0;
            foreach (var notification in notifications.Where(x => !x.IsRead))
            {
                notification.Read();
                updatedCount++;
            }

            if (updatedCount > 0)
                await _unitOfWork.CommitAsync();

            return new MarkNotificationsAsReadResultDTO { UpdatedCount = updatedCount };
        }
    }
}
