using UserApi.Domain.Entities;

namespace UserApi.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
}
