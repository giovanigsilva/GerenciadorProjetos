using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Interfaces.Services;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUsuarioService usuarioService, ILogger<AuthController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        /// <summary>
        /// Realiza o login do usuário e retorna um token JWT.
        /// </summary>
        /// <param name="dto">Dados de login contendo email e senha.</param>
        /// <returns>Token JWT para autenticação.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
            {
                _logger.LogWarning(MensagemApi.DadosInvalidos.GetMensagem());
                return BadRequest(new { message = MensagemApi.DadosInvalidos.GetMensagem() });
            }

            try
            {
                var token = await _usuarioService.LoginAsync(dto.Email, dto.Senha);

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Falha de login para o email {Email}.", dto.Email);
                    return Unauthorized(new { message = MensagemApi.LoginFalha.GetMensagem() });
                }

                _logger.LogInformation("Usuário {Email} autenticado com sucesso.", dto.Email);

                return Ok(new
                {
                    message = MensagemApi.LoginSucesso.GetMensagem(),
                    token = token
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Acesso não autorizado para o usuário {Email}.", dto.Email);
                return Unauthorized(new { message = MensagemApi.AcessoNaoAutorizado.GetMensagem() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao realizar login do usuário {Email}.", dto?.Email);
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }
    }
}
