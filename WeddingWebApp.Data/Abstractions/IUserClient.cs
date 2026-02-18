using WeddingWebApp.Data.Models;

namespace WeddingWebApp.Data.Abstractions
{
    public interface IUserClient
    {
        Task<IReadOnlyCollection<User>> GetAllAsync();
        Task<User?> GetByIdAsync(string id);
        Task<User> CreateAsync(User user);
        Task<User?> UpdateAsync(string id, User update);
        Task<bool> DeleteAsync(string id);
    }
}

