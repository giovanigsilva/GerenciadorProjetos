using ProjectManagement.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Interfaces.Repositories
{
    public interface IProjetoRepository
    {
        Task<Projeto> GetByIdAsync(int id);
        Task<IEnumerable<Projeto>> GetAllByUsuarioAsync(int usuarioId);
        Task AddAsync(Projeto projeto);
        void Update(Projeto projeto);
        void Remove(Projeto projeto);
        Task<bool> TemTarefasPendentesAsync(int projetoId);
    }
}
