using ChatALot.Data.Models.Domains;
using ChatALot.Data.Models.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatALot.Data.Services.Interface
{
    public interface IUserServices
    {
        Task<IEnumerable<ReadUserRequest>> GetAllUsers();
        Task<ReadUserRequest> GetByUserId(Guid id);
        Task<string> AddUser(CreateUserRequest request);
        Task<string> UpdateUser(UpdateUserRequest request);
        Task<string> DeleteUser(DeleteUserRequest request);

        //Auth
        Task<LoginResponse> Login(LoginRequest request);
    }
}
