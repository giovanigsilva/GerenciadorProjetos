using ProjectManagement.Domain.Entities;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObterPorIdAsync(int id);
        Task<Usuario> ObterPorEmailAsync(string email);
        Task AdicionarAsync(Usuario usuario);
        Task<Usuario> ObterPorHashDoTokenDeRecuperacaoAsync(string tokenHash);
        void Atualizar(Usuario usuario);
        void Remover(Usuario usuario);
    }
}
