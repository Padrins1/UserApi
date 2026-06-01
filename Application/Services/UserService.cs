using UserApi.Application.DTOs;
using UserApi.Application.Interfaces;
using UserApi.Domain.Interfaces;

namespace UserApi.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return null;

        return new UserResponseDto
        {
            Id          = user.Id,
            FullName    = user.FullName,
            Email       = user.Email.ToLower(),
            Domain      = ExtractDomain(user.Email),
            Username    = user.Username.ToLower(),
            Phone       = FormatPhone(user.Phone),
            Website     = NormalizeWebsite(user.Website),
            CompanyName = user.CompanyName
        };
    }

    private static string ExtractDomain(string email)
    {
        var parts = email.Split('@');
        return parts.Length == 2 ? parts[1].ToLower() : string.Empty;
    }

    private static string FormatPhone(string phone)
    {
        return new string(phone.Where(char.IsDigit).ToArray());
    }

    private static string NormalizeWebsite(string website)
    {
        if (string.IsNullOrWhiteSpace(website))
            return string.Empty;

        if (!website.StartsWith("http://") && !website.StartsWith("https://"))
            return $"https://{website}";

        return website.Replace("http://", "https://");
    }
}
