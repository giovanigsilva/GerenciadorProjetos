using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Enums;
using ProjectManagement.Domain.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComentariosController : ControllerBase
    {
        private readonly IComentarioService _comentarioService;
        private readonly ILogger<ComentariosController> _logger;

        public ComentariosController(
            IComentarioService comentarioService,
            ILogger<ComentariosController> logger)
        {
            _comentarioService = comentarioService;
            _logger = logger;
        }

        /// <summary>
        /// Adiciona um comentário via stored procedure.
        /// </summary>
        [HttpPost("sp")]
        public async Task<IActionResult> AdicionarComentarioViaStoredProcedure([FromBody] AdicionarComentarioDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning(MensagemApi.DadosInvalidos.GetMensagem());
                return BadRequest(new { message = MensagemApi.DadosInvalidos.GetMensagem() });
            }

            try
            {
                await _comentarioService.AdicionarComentarioViaStoredProcedureAsync(dto);

                _logger.LogInformation(
                    "Comentário adicionado via SP por UsuarioID: {UsuarioID} na TarefaID: {TarefaID}.",
                    dto.UsuarioID, dto.TarefaID);

                return Ok(new { message = MensagemApi.ComentarioViaSpSucesso.GetMensagem() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro ao adicionar comentário via SP (UsuarioID: {UsuarioID}, TarefaID: {TarefaID})",
                    dto?.UsuarioID, dto?.TarefaID);

                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }

        /// <summary>
        /// Adiciona um comentário diretamente no banco.
        /// </summary>
        [HttpPost("db")]
        public async Task<IActionResult> AdicionarComentarioDiretoNoBanco([FromBody] AdicionarComentarioDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning(MensagemApi.DadosInvalidos.GetMensagem());
                return BadRequest(new { message = MensagemApi.DadosInvalidos.GetMensagem() });
            }

            try
            {
                await _comentarioService.AdicionarComentarioDiretoAsync(dto);

                _logger.LogInformation(
                    "Comentário adicionado diretamente no banco por UsuarioID: {UsuarioID} na TarefaID: {TarefaID}.",
                    dto.UsuarioID, dto.TarefaID);

                return Ok(new { message = MensagemApi.ComentarioDiretoSucesso.GetMensagem() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro ao adicionar comentário direto (UsuarioID: {UsuarioID}, TarefaID: {TarefaID})",
                    dto?.UsuarioID, dto?.TarefaID);

                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }
    }
}
