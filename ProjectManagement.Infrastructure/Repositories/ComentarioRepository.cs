using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces.Repositories;
using ProjectManagement.Infrastructure.Data;
using System;
using System.Data;
using System.Threading.Tasks;

namespace ProjectManagement.Infrastructure.Repositories
{
    public class ComentarioRepository : IComentarioRepository
    {
        private readonly AppDbContext _context;

        public ComentarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarComentarioViaStoredProcedureAsync(AdicionarComentarioDto dto)
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
        }
    }
}
