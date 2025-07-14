using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Domain.Interfaces.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize(Roles = "gerente")] // Garante acesso apenas a gerentes
    public class RelatoriosController : ControllerBase
    {
        private readonly IRelatorioService _relatorioService;

        public RelatoriosController(IRelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        /// <summary>
        /// Obtém a média de tarefas concluídas por usuário nos últimos 30 dias.
        /// </summary>
        /// <returns>Média das tarefas concluídas.</returns>
        /// <response code="200">Média calculada com sucesso.</response>
        [HttpGet("media-tarefas-concluidas")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> ObterMediaTarefasConcluidas()
        {
            //var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            //if (usuarioIdClaim == null)
            //    return Unauthorized();

            //if (!int.TryParse(usuarioIdClaim.Value, out int usuarioId))
            //    return Unauthorized();

           // var media = await _relatorioService.RelatorioDesempenhoAsync(usuarioId);
            var media = await _relatorioService.RelatorioDesempenhoAsync(1002);
            return Ok(new { MediaTarefasConcluidas = media });
        }
    }
}
