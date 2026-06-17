using Microsoft.AspNetCore.SignalR;

namespace SmartGrid.WebApi.Hubs
{
    public class DeviceHub : Hub
    {
        public Task JoinPropertyGroup(string propertyId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, DeviceHubGroups.Property(propertyId));
        }

        public Task LeavePropertyGroup(string propertyId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, DeviceHubGroups.Property(propertyId));
        }
    }
}
