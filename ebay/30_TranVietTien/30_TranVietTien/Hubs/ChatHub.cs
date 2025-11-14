using Microsoft.AspNetCore.SignalR;

namespace _30_TranVietTien.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public async Task JoinRoom(string room)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, room);
                _logger.LogInformation($"User {Context.ConnectionId} joined room {room}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error joining room {room}");
                throw;
            }
        }

        public async Task LeaveRoom(string room)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
                _logger.LogInformation($"User {Context.ConnectionId} left room {room}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error leaving room {room}");
                throw;
            }
        }

        public async Task SendMessage(string room, int senderId, int receiverId, string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    _logger.LogWarning("Attempted to send empty message");
                    return;
                }

                // Format thời gian theo định dạng Việt Nam
                var time = DateTime.Now.ToString("HH:mm dd/MM/yyyy");

                _logger.LogInformation($"Broadcasting message from {senderId} to {receiverId} in room {room}: {message}");

                // Gửi tin nhắn tới tất cả members trong room
                await Clients.Group(room).SendAsync(
                    "ReceiveMessage",
                    senderId,
                    receiverId,
                    message,
                    time
                );

                _logger.LogInformation($"Message broadcasted successfully to room {room}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending message in room {room} from {senderId} to {receiverId}");
                throw;
            }
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"User {Context.ConnectionId} connected to ChatHub");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                _logger.LogError(exception, $"User {Context.ConnectionId} disconnected with error");
            }
            else
            {
                _logger.LogInformation($"User {Context.ConnectionId} disconnected normally");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
