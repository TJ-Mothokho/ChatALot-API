using ChatALot.Data.Models.Domains;
using ChatALot.Data.Models.DTOs.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatALot.Data.Services.Interface
{
    public interface IMessageServices
    {
        Task<bool> SendMessage(SendMessageRequest message);
        Task<IEnumerable<ReceiveMessageRequest>> ReceiveMessage(Guid id);
    }
}
