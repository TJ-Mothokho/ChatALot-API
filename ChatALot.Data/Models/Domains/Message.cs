using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatALot.Data.Models.Domains
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Id))]
        public Guid SenderId { get; set; } // Foreign key to User

        [ForeignKey(nameof(Id))]
        public Guid ReceiverId { get; set; } // Foreign key to User
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}
