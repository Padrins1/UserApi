using Microsoft.AspNetCore.Mvc;
using UserApi.Application.Interfaces;
using UserApi.Infrastructure;

namespace UserApi.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>Retorna os dados transformados de um usuário pelo seu ID.</summary>
    /// <param name="id">ID numérico do usuário (1 a 10 disponíveis no JSONPlaceholder)</param>
    /// <response code="200">Usuário encontrado</response>
    /// <response code="400">ID inválido</response>
    /// <response code="404">Usuário não encontrado</response>
    /// <response code="503">API externa indisponível</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest(new { error = "O ID deve ser um número inteiro positivo." });

        try
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user is null)
                return NotFound(new { error = $"Usuário com ID {id} não encontrado." });

            return Ok(user);
        }
        catch (ExternalApiException ex)
        {
            _logger.LogWarning("Falha na API externa ao buscar ID {Id}: {Message}", id, ex.Message);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = ex.Message });
        }
    }
}
