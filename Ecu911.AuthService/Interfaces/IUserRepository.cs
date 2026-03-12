using Ecu911.AuthService.Models;

namespace Ecu911.AuthService.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdAsync(Guid id);
    Task<List<User>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task SaveChangesAsync();
}