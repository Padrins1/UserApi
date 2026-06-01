using System.Text.Json;
using UserApi.Domain.Entities;
using UserApi.Domain.Interfaces;

namespace UserApi.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IHttpClientFactory httpClientFactory, ILogger<UserRepository> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("JsonPlaceholder");
            var response = await client.GetAsync($"users/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var externalUser = JsonSerializer.Deserialize<ExternalUserModel>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (externalUser is null)
                return null;

            return new User
            {
                Id          = externalUser.Id,
                FullName    = externalUser.Name,
                Email       = externalUser.Email,
                Username    = externalUser.Username,
                Phone       = externalUser.Phone,
                Website     = externalUser.Website,
                CompanyName = externalUser.Company?.Name ?? string.Empty
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro ao chamar a API externa para o ID {Id}", id);
            throw new ExternalApiException("Não foi possível conectar à API externa. Tente novamente mais tarde.");
        }
    }
}

internal class ExternalUserModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public ExternalCompanyModel? Company { get; set; }
}

internal class ExternalCompanyModel
{
    public string Name { get; set; } = string.Empty;
}

public class ExternalApiException : Exception
{
    public ExternalApiException(string message) : base(message) { }
}
