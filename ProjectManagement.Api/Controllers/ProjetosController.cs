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
        [HttpGet("{usuarioId}")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> ListarPorUsuario(int usuarioId)
        {
            try
            {
                var projetos = await _projetoService.GetAllByUsuarioAsync(usuarioId);

                if (projetos == null || !projetos.Any())
                {
                    _logger.LogInformation("Nenhum projeto encontrado para o usuário {UsuarioId}.", usuarioId);
                    return Ok(new
                    {
                        message = "Nenhum projeto encontrado para este usuário.",
                        projetos = Array.Empty<CriarProjetoDto>()
                    });
                }

                _logger.LogInformation("Projetos listados com sucesso para o usuário {UsuarioId}.", usuarioId);
                return Ok(new
                {
                    message = MensagemApi.ListaSucesso.GetMensagem(),
                    projetos
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar projetos do usuário {UsuarioID}.", usuarioId);
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }

        /// <summary>
        /// Cria um novo projeto.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(object), 201)]
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
                _logger.LogInformation("Projeto criado com sucesso [Projeto: {Nome} - UsuarioID: {Id}].",
                                       projeto.Nome, projeto.UsuarioID);

                return CreatedAtAction(nameof(ListarPorUsuario), new { usuarioId = projeto.UsuarioID }, new
                {
                    message = MensagemApi.CriacaoSucesso.GetMensagem(),
                    projeto
                });
            }
            catch (Exception ex) when (ex.Message == "Usuário não encontrado.")
            {
                _logger.LogWarning("Usuário não encontrado ao criar projeto. DTO: {@Dto}", dto);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar projeto. DTO: {@Dto}", dto);
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }

        /// <summary>
        /// Atualiza um projeto existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarProjetoDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Tentativa de atualização com DTO nulo. ID: {Id}", id);
                return BadRequest(new { message = MensagemApi.DadosInvalidos.GetMensagem() });
            }

            try
            {
                dto.Id = id;
                await _projetoService.AtualizarProjetoAsync(dto);
                _logger.LogInformation("Projeto atualizado com sucesso. ID: {Id}", id);
                return Ok(new { message = MensagemApi.AtualizacaoSucesso.GetMensagem() });
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
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Deletar(int id)
        {
            try
            {
                await _projetoService.DeletarProjetoAsync(id);
                _logger.LogInformation("Projeto deletado com sucesso. ID: {Id}", id);
                return Ok(new { message = MensagemApi.RemocaoSucesso.GetMensagem() });
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
