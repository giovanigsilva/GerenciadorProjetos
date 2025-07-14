using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces.Repositories;
using ProjectManagement.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagement.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> ObterPorIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario> ObterPorEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AdicionarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
        }

        public async Task<Usuario> ObterPorHashDoTokenDeRecuperacaoAsync(string tokenHash)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.TokenRecuperacaoSenha == tokenHash);
        }

        public void Atualizar(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
        }

        public void Remover(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
        }
    }
}
