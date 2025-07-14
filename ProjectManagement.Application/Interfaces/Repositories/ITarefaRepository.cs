using ProjectManagement.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Interfaces.Repositories
{
    public interface ITarefaRepository
    {
        Task<Tarefa> ObterPorIdAsync(int id);
        Task<IEnumerable<Tarefa>> ObterTodasPorProjetoAsync(int projetoId);
        Task AdicionarAsync(Tarefa tarefa);
        void Atualizar(Tarefa tarefa);
        void Remover(Tarefa tarefa);
        Task<List<Tarefa>> ObterConcluidasUltimos30DiasAsync();
    }
}
