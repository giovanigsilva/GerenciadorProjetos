using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Interfaces.Services;
using System.Threading.Tasks;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Realiza o login do usuário e retorna um token JWT.
        /// </summary>
        /// <param name="dto">Dados de login contendo email e senha.</param>
        /// <returns>Token JWT para autenticação.</returns>
        /// <response code="200">Login realizado com sucesso.</response>
        /// <response code="400">Dados de entrada inválidos.</response>
        /// <response code="401">Falha na autenticação.</response>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _usuarioService.LoginAsync(dto.Email, dto.Senha);
            if (token == null)
                return Unauthorized();
            return Ok(new { Token = token });
        }
    }
}
