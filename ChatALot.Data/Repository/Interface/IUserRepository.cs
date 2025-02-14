using ChatALot.Data.Models.Domains;

namespace ChatALot.Data.Repository.Interface
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(Guid id);
        Task<string> AddAsync(User request);
        Task<string> UpdateAsync(User request);
        Task<string> DeleteAsync(User request);

        //Auth
        Task<User> GetByUsernameAndPassword(User user);
    }
}
