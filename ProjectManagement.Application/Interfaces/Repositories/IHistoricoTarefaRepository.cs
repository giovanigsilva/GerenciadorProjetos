using ProjectManagement.Domain.Entities;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Interfaces.Repositories
{
    public interface IHistoricoTarefaRepository
    {
        Task AdicionarAsync(HistoricoTarefa historico);
    }
}
