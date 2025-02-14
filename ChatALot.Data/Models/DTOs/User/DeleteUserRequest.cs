using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatALot.Data.Models.DTOs.User
{
    public class DeleteUserRequest
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
    }
}
