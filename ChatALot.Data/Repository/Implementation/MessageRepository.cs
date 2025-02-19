using ChatALot.Data.Context;
using ChatALot.Data.Models.Domains;
using ChatALot.Data.Repository.Interface;
using ChatALot.Data.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatALot.Data.Repository.Implementation
{
    public class MessageRepository : Hub, IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SendAsync(Message message)
        {
            try
            {
                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                await Clients.All.SendAsync("ReceiveMessage", message.SenderId, message.ReceiverId, message.Content);

                Log.Information($"{message.SenderId} texted {message.ReceiverId}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Message>> ReceiveAsync(Guid user1, Guid user2)
        {
            return await _context.Messages
                .Where(message => (message.SenderId == user1 && message.ReceiverId == user2) ||
                                  (message.SenderId == user2 && message.ReceiverId == user1))
                .OrderBy(message => message.SentAt)
                .ToListAsync();
        }
    }
}
