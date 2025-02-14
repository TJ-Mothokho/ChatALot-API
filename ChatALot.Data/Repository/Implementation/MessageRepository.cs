using ChatALot.Data.Context;
using ChatALot.Data.Models.Domains;
using ChatALot.Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatALot.Data.Repository.Implementation
{
    public class MessageRepository : IMessageRepository
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

                Log.Information($"{message.SenderId} texted {message.ReceiverId}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Message>> ReceiveAsync(Guid id)
        {
            return await _context.Messages.Where(message => message.SenderId == id && message.ReceiverId == id).ToListAsync();
        }
    }
}
