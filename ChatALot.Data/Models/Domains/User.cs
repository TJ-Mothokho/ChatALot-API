using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatALot.Data.Models.Domains
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } // Store hashed password
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for messages sent by the user
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
