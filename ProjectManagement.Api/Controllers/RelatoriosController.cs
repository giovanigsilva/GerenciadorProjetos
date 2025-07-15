using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectManagement.Domain.Interfaces.Services;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "gerente")] // Garante acesso apenas a gerentes
    public class RelatoriosController : ControllerBase
    {
        private readonly IRelatorioService _relatorioService;
        private readonly ILogger<RelatoriosController> _logger;

        public RelatoriosController(IRelatorioService relatorioService, ILogger<RelatoriosController> logger)
        {
            _relatorioService = relatorioService;
            _logger = logger;
        }

        /// <summary>
        /// Obtém a média de tarefas concluídas por usuário nos últimos 30 dias.
        /// </summary>
        /// <returns>Média das tarefas concluídas.</returns>
        /// <response code="200">Média calculada com sucesso.</response>
        /// <response code="500">Erro interno ao calcular média.</response>
        [HttpGet("media-tarefas-concluidas")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ObterMediaTarefasConcluidas()
        {
            try
            {
                // Para futuro: uso do usuário autenticado (mantenha comentado se desejar)
                // var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                // if (usuarioIdClaim == null)
                // {
                //     _logger.LogWarning("Usuário não autenticado ao acessar relatório.");
                //     return Unauthorized();
                // }
                // if (!int.TryParse(usuarioIdClaim.Value, out int usuarioId))
                // {
                //     _logger.LogWarning("Falha ao converter Claim de usuário no relatório.");
                //     return Unauthorized();
                // }
                // var media = await _relatorioService.RelatorioDesempenhoAsync(usuarioId);

                // Usuário fixo (exemplo para testes):
                var usuarioId = 1002;
                var media = await _relatorioService.RelatorioDesempenhoAsync(usuarioId);

                _logger.LogInformation("Relatório de média de tarefas concluídas obtido para usuário {UsuarioId}.", usuarioId);
                return Ok(new { MediaTarefasConcluidas = media });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter relatório de média de tarefas concluídas.");
                return StatusCode(500, new { message = "Erro interno ao processar o relatório." });
            }
        }
    }
}
