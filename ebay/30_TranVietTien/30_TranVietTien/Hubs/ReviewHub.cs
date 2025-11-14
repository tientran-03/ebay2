using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace _30_TranVietTien.Hubs
{
    public class ReviewHub : Hub
    {
        public async Task JoinProductGroup(string productId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, productId);
        }

        public async Task LeaveProductGroup(string productId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, productId);
        }
    }
}
