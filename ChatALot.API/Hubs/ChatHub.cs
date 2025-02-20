using Microsoft.AspNetCore.SignalR;

namespace ChatALot.API.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(Guid senderId, Guid receiverId, string content)
        {
            await Clients.All.SendAsync("ReceiveMessage", senderId, receiverId, content);
        }
    }
}
