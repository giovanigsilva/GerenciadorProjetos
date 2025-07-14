using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces.Repositories;
using ProjectManagement.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Infrastructure.Repositories
{
    public class ProjetoRepository : IProjetoRepository
    {
        private readonly AppDbContext _context;

        public ProjetoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Projeto> GetByIdAsync(int id)
        {
            return await _context.Projetos
                .Include(p => p.Tarefas)
                .FirstOrDefaultAsync(p => p.ProjetoID == id);
        }

        public async Task<IEnumerable<Projeto>> GetAllByUsuarioAsync(int usuarioId)
        {
            return await _context.Projetos
                .Where(p => p.UsuarioID == usuarioId)
                .Include(p => p.Tarefas)
                .ToListAsync();
        }

        public async Task AddAsync(Projeto projeto)
        {
            await _context.Projetos.AddAsync(projeto);
        }

        public void Update(Projeto projeto)
        {
            _context.Projetos.Update(projeto);
        }

        public void Remove(Projeto projeto)
        {
            _context.Projetos.Remove(projeto);
        }

        public async Task<bool> TemTarefasPendentesAsync(int projetoId)
        {
            return await _context.Tarefas
                .AnyAsync(t => t.ProjetoID == projetoId && t.Status == "pendente");
        }
    }
}
