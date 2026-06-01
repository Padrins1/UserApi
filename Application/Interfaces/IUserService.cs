using UserApi.Application.DTOs;

namespace UserApi.Application.Interfaces;

public interface IUserService
{
    Task<UserResponseDto?> GetUserByIdAsync(int id);
}
