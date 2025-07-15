using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs.Projeto;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjetosController : ControllerBase
    {
        private readonly IProjetoService _projetoService;
        private readonly ILogger<ProjetosController> _logger;

        public ProjetosController(
            IProjetoService projetoService,
            ILogger<ProjetosController> logger)
        {
            _projetoService = projetoService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os projetos de um usuário.
        /// </summary>
        /// <param name="usuarioId">ID do usuário.</param>
        [HttpGet("{usuarioId}")]
        [ProducesResponseType(typeof(IEnumerable<CriarProjetoDto>), 200)]
        public async Task<IActionResult> ListarPorUsuario(int usuarioId)
        {
            try
            {
                var projetos = await _projetoService.GetAllByUsuarioAsync(usuarioId);
                return Ok(projetos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar projetos do usuário {UsuarioID}", usuarioId);
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }

        /// <summary>
        /// Cria um novo projeto.
        /// </summary>
        /// <param name="dto">Dados do projeto a ser criado.</param>
        [HttpPost]
        [ProducesResponseType(typeof(CriarProjetoDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Criar([FromBody] CriarProjetoDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Tentativa de criação com DTO nulo.");
                return BadRequest(new { message = MensagemApi.DadosInvalidos.GetMensagem() });
            }

            try
            {
                var projeto = await _projetoService.CriarProjetoAsync(dto);
                _logger.LogInformation("Projeto criado com sucesso: {Projeto}", projeto.Nome);

                return CreatedAtAction(nameof(ListarPorUsuario), new { usuarioId = projeto.Id }, projeto);
            }
            catch (Exception ex) when (ex.Message == "Usuário não encontrado.")
            {
                _logger.LogWarning("Usuário não encontrado ao criar projeto. DTO: {@Dto}", dto);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar projeto. DTO: {@Dto}", dto);
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }

        /// <summary>
        /// Atualiza um projeto existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarProjetoDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Tentativa de atualização com DTO nulo.");
                return BadRequest(new { message = MensagemApi.DadosInvalidos.GetMensagem() });
            }

            try
            {
                dto.Id = id;
                await _projetoService.AtualizarProjetoAsync(dto);
                _logger.LogInformation("Projeto atualizado com sucesso. ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message == "Projeto não encontrado.")
            {
                _logger.LogWarning("Projeto não encontrado para atualização. ID: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar projeto. ID: {Id}", id);
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }

        /// <summary>
        /// Deleta um projeto pelo ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Deletar(int id)
        {
            try
            {
                await _projetoService.DeletarProjetoAsync(id);
                _logger.LogInformation("Projeto deletado com sucesso. ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex) when (ex.Message == "Projeto não encontrado.")
            {
                _logger.LogWarning("Projeto não encontrado para exclusão. ID: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar projeto. ID: {Id}", id);
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }
    }
}
