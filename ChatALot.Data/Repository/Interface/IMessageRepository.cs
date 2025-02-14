using ChatALot.Data.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatALot.Data.Repository.Interface
{
    public interface IMessageRepository
    {
        Task<bool> SendAsync(Message message);
        Task<IEnumerable<Message>> ReceiveAsync(Guid id);
    }
}
