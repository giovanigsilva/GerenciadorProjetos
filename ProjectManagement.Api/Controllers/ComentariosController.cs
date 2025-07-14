using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComentariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComentariosController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adiciona um comentário via stored procedure.
        /// </summary>
        /// <param name="dto">Dados do comentário a ser adicionado.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Comentário adicionado com sucesso.</response>
        /// <response code="500">Erro interno ao adicionar comentário.</response>
        [HttpPost("sp")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AdicionarComentarioViaStoredProcedure([FromBody] AdicionarComentarioDto dto)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@UsuarioID", dto.UsuarioID),
                    new SqlParameter("@TarefaID", dto.TarefaID),
                    new SqlParameter("@Comentario", dto.Comentario ?? (object)DBNull.Value)
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_AdicionarComentario @UsuarioID, @TarefaID, @Comentario",
                    parameters);

                return Ok(new { message = "Comentário adicionado com sucesso via stored procedure." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao adicionar comentário: {ex.Message}" });
            }
        }

        /// <summary>
        /// Adiciona um comentário diretamente no banco.
        /// </summary>
        /// <param name="dto">Dados do comentário a ser adicionado.</param>
        /// <returns>Mensagem de sucesso.</returns>
        /// <response code="200">Comentário adicionado com sucesso.</response>
        [HttpPost("db")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> AdicionarComentarioDiretoNoBanco([FromBody] AdicionarComentarioDto dto)
        {
            var historico = new HistoricoTarefa
            {
                TarefaID = dto.TarefaID,
                UsuarioID = dto.UsuarioID,
                DescricaoAlteracao = $"Comentário adicionado: {dto.Comentario}",
                DataAlteracao = DateTime.UtcNow
            };

            _context.HistoricoTarefas.Add(historico);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Comentário adicionado diretamente no banco." });
        }
    }
}
