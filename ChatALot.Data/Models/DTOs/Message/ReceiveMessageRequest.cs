using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatALot.Data.Models.DTOs.Message
{
    public class ReceiveMessageRequest
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; } 
        public Guid ReceiverId { get; set; }
        public string Content { get; set; }
    }
}
