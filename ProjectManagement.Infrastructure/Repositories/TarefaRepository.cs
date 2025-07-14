using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces.Repositories;
using ProjectManagement.Infrastructure.Data;

namespace ProjectManagement.Infrastructure.Repositories
{
    public class TarefaRepository : ITarefaRepository
    {
        private readonly AppDbContext _context;

        public TarefaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tarefa> ObterPorIdAsync(int id)
        {
            return await _context.Tarefas
                .Include(t => t.Projeto)
                    .ThenInclude(p => p.Usuario)
                .FirstOrDefaultAsync(t => t.TarefaID == id);
        }

        public async Task<IEnumerable<Tarefa>> ObterTodasPorProjetoAsync(int projetoId)
        {
            return await _context.Tarefas
                .Where(t => t.ProjetoID == projetoId)
                .ToListAsync();
        }

        public async Task AdicionarAsync(Tarefa tarefa)
        {
            await _context.Tarefas.AddAsync(tarefa);
        }

        public void Atualizar(Tarefa tarefa)
        {
            _context.Tarefas.Update(tarefa);
        }

        public void Remover(Tarefa tarefa)
        {
            _context.Tarefas.Remove(tarefa);
        }

        public async Task<List<Tarefa>> ObterConcluidasUltimos30DiasAsync()
        {
            var dataLimite = DateTime.UtcNow.AddDays(-30);

            return await _context.Tarefas
                .Where(t => t.Status == "concluída" && t.DataCriacao >= dataLimite)
                .ToListAsync();
        }
    }
}
