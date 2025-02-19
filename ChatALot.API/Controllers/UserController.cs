using Azure.Core;
using ChatALot.Data.Models.DTOs.User;
using ChatALot.Data.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ChatALot.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _user;

        public UserController(IUserServices userServices)
        {
            _user = userServices;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsernames()
        {
            try
            {
                Log.Information("Attempting to get all users");
                
                var response = await _user.GetAllUsers();
                return Ok(response);
            }
            catch(Exception ex)
            {
                Log.Error(ex, ex.Message, ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                Log.Information($"Attempting to get user by ID {id}");

                var response = await _user.GetByUserId(id);
                return Ok(response);
            }
            catch(Exception ex)
            {
                Log.Error(ex, ex.Message, ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            try
            {
                Log.Information($"Attempting to update user {request.Id}");

                var response = await _user.UpdateUser(request);

                return Ok(response);
            }
            catch(Exception ex)
            {
                Log.Error(ex, ex.Message, ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                Log.Information($"Attempting to delete user {id}");

                var response = await _user.DeleteUser(id);

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }
    }
}
