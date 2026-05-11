using EchoProject.Application.Common;
using EchoProject.Application.DTO;
using EchoProject.Application.Requests.Notifications;
using EchoProject.Domain.Interfaces;
using EchoProject.Domain.Notifications;

namespace EchoProject.Application.Services
{
    [AppService]
    public class PushDeviceService(IUnitOfWork unitOfWork)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task RegisterAsync(UserDTO user, RegisterPushDeviceRequest request, CancellationToken ct = default)
        {
            var token = request.Token.Trim();
            var platform = request.Platform.Trim();
            var existingDevice = await _unitOfWork.PushDevices.FindByTokenAsync(token, ct);

            if (existingDevice is null)
            {
                var device = new PushDevice(user.Id, token, platform);
                await _unitOfWork.PushDevices.AddAsync(device, ct);
            }
            else
            {
                existingDevice.Refresh(user.Id, platform);
            }

            await _unitOfWork.CommitAsync(ct);
        }

        public async Task RemoveAsync(UserDTO user, RemovePushDeviceRequest request, CancellationToken ct = default)
        {
            var device = await _unitOfWork.PushDevices.FindByTokenAsync(request.Token.Trim(), ct);
            if (device is null || device.UserId != user.Id)
                return;

            device.Deactivate();
            await _unitOfWork.CommitAsync(ct);
        }
    }
}
