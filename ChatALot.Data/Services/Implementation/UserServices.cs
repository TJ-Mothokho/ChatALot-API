﻿using ChatALot.Data.Models.DTOs.User;
using ChatALot.Data.Repository.Interface;
using ChatALot.Data.Services.Interface;
using ChatALot.Data.Models.Domains;
using Serilog;
using Azure.Core;

namespace ChatALot.Data.Services.Implementation
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        public UserServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> AddUser(CreateUserRequest request)
        {
            try
            {
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    Status = "Active"
                };
                
                var response = await _userRepository.AddAsync(user);
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> DeleteUser(DeleteUserRequest request)
        {
            try
            {
                var user = new User
                {
                    Id = request.Id,
                    Status = "Active"
                };

                var response = await _userRepository.DeleteAsync(user);
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public  async Task<IEnumerable<ReadUserRequest>> GetAllUsers()
        {
            try
            {
                var request = await _userRepository.GetAllAsync();

                var response = new List<ReadUserRequest>();

                foreach(var user in request)
                {
                    var userResponse = new ReadUserRequest
                    {
                        Id=user.Id,
                        Username = user.Username,
                        Email = user.Email
                    };

                    response.Add(userResponse);
                }

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ReadUserRequest> GetByUserId(Guid id)
        {
            try
            {
                var request = await _userRepository.GetByIdAsync(id);

                var response = new ReadUserRequest();

                if (request is not null)
                {
                    response.Id = request.Id;
                    response.Username = request.Username;
                    response.Email = request.Email;                }
                else
                {
                    throw new NullReferenceException();
                }

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<string> UpdateUser(UpdateUserRequest request)
        {
            try
            {
                var user = new User
                {
                    Id = request.Id,
                    Username = request.Username,
                    Password = request.Password
                };

                var response = await _userRepository.UpdateAsync(user);
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
