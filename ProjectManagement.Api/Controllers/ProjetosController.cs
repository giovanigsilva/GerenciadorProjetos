using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs.Projeto;
using System.Threading.Tasks;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjetosController : ControllerBase
    {
        private readonly IProjetoService _projetoService;

        public ProjetosController(IProjetoService projetoService)
        {
            _projetoService = projetoService;
        }

        /// <summary>
        /// Lista todos os projetos de um usuário.
        /// </summary>
        /// <param name="usuarioId">ID do usuário.</param>
        /// <returns>Lista de projetos.</returns>
        /// <response code="200">Lista de projetos retornada com sucesso.</response>
        [HttpGet("{usuarioId}")]
        [ProducesResponseType(typeof(IEnumerable<CriarProjetoDto>), 200)]
        public async Task<IActionResult> ListarPorUsuario(int usuarioId)
        {
            var projetos = await _projetoService.GetAllByUsuarioAsync(usuarioId);
            return Ok(projetos);
        }

        /// <summary>
        /// Cria um novo projeto.
        /// </summary>
        /// <param name="dto">Dados do projeto a ser criado.</param>
        /// <returns>Projeto criado.</returns>
        /// <response code="201">Projeto criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CriarProjetoDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Criar([FromBody] CriarProjetoDto dto)
        {
            try
            {
                var projeto = await _projetoService.CriarProjetoAsync(dto);
                return CreatedAtAction(nameof(ListarPorUsuario), new { usuarioId = projeto.Id }, projeto);
            }
            catch (Exception ex) when (ex.Message == "Usuário não encontrado.")
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um projeto existente.
        /// </summary>
        /// <param name="id">ID do projeto.</param>
        /// <param name="dto">Dados para atualização do projeto.</param>
        /// <response code="204">Projeto atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="404">Projeto não encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarProjetoDto dto)
        {
            try
            {
                dto.Id = id;
                await _projetoService.AtualizarProjetoAsync(dto);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message == "Projeto não encontrado.")
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um projeto pelo ID.
        /// </summary>
        /// <param name="id">ID do projeto.</param>
        /// <response code="204">Projeto deletado com sucesso.</response>
        /// <response code="404">Projeto não encontrado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Deletar(int id)
        {
            try
            {
                await _projetoService.DeletarProjetoAsync(id);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message == "Projeto não encontrado.")
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
