using Azure.Core;
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
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Where(user => user.Status == "Active")
                .ToListAsync();
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Where(user => user.Id == id && user.Status == "Active")
                .FirstOrDefaultAsync();
        }

        public async Task<string> AddAsync(User request)
        {
            try
            {
                await _context.Users.AddAsync(request);
                await _context.SaveChangesAsync();

                Log.Information($"New account: {request.Username} created!!");
                return "Account created successfully";
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message );
                throw;
            }
        }

        public async Task<string> UpdateAsync(User request)
        {
            try
            {
                _context.Users.Update(request);
                await _context.SaveChangesAsync();

                Log.Information($"{request.Id} updated profile!!");
                return "Account updated successfully";
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        public async Task<string> DeleteAsync(User request)
        {
            try
            {
                _context.Users.Update(request);
                await _context.SaveChangesAsync();

                Log.Information($"Account {request.Username} deleted!!");
                return "Account deleted!";
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        //Auth
        public async Task<User> GetByUsernameAndPassword(User user)
        {
            try
            {
                var response = await _context.Users
                    .Where(person => person.Username == user.Username || person.Email == user.Email && person.Password == user.Password && person.Status == "Active")
                    .FirstOrDefaultAsync();

                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }
    }
}
