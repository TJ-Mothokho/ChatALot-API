using ChatALot.Data.Context;
using ChatALot.Data.Models.Domains;
using Microsoft.AspNetCore.SignalR;

namespace ChatALot.Data.Services
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(Guid senderId, Guid receiverId, string message)
        {
            var newMessage = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", senderId, receiverId, message);
        }
    }
}
