using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Interfaces.Services;
using System.Threading.Tasks;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <param name="dto">Dados do usuário a ser criado.</param>
        /// <returns>Usuário criado.</returns>
        /// <response code="201">Usuário criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CriarUsuarioDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Criar([FromBody] CriarUsuarioDto dto)
        {
            var usuario = await _usuarioService.CriarUsuarioAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
        }

        /// <summary>
        /// Obtém um usuário pelo ID.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <returns>Usuário encontrado.</returns>
        /// <response code="200">Usuário retornado com sucesso.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CriarUsuarioDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var usuario = await _usuarioService.ObterUsuarioPorIdAsync(id);
            if (usuario == null)
                return NotFound();
            return Ok(usuario);
        }

        /// <summary>
        /// Obtém um usuário pelo email.
        /// </summary>
        /// <param name="email">Email do usuário.</param>
        /// <returns>Usuário encontrado.</returns>
        /// <response code="200">Usuário retornado com sucesso.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpGet("email")]
        [ProducesResponseType(typeof(CriarUsuarioDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObterPorEmail([FromQuery] string email)
        {
            var usuario = await _usuarioService.ObterUsuarioPorEmailAsync(email);
            if (usuario == null)
                return NotFound();
            return Ok(usuario);
        }

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <param name="dto">Dados para atualização do usuário.</param>
        /// <response code="204">Usuário atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarUsuarioDto dto)
        {
            dto.Id = id;
            await _usuarioService.AtualizarUsuarioAsync(dto);
            return NoContent();
        }

        /// <summary>
        /// Deleta um usuário pelo ID.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <response code="204">Usuário deletado com sucesso.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Deletar(int id)
        {
            await _usuarioService.DeletarUsuarioAsync(id);
            return NoContent();
        }
    }
}
