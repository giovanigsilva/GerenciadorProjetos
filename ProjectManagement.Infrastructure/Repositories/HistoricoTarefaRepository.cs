using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces.Repositories;
using ProjectManagement.Infrastructure.Data;
using System.Threading.Tasks;

namespace ProjectManagement.Infrastructure.Repositories
{
    public class HistoricoTarefaRepository : IHistoricoTarefaRepository
    {
        private readonly AppDbContext _context;

        public HistoricoTarefaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(HistoricoTarefa historico)
        {
            await _context.HistoricoTarefas.AddAsync(historico);
        }
    }
}
