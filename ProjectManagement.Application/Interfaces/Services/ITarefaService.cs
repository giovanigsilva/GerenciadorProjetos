using ProjectManagement.Application.DTOs;

namespace ProjectManagement.Domain.Interfaces.Services
{
    public interface ITarefaService
    {
        Task<IEnumerable<TarefaDto>> ObterTodasPorProjetoAsync(int projetoId);

        Task AdicionarTarefaAsync(CriarTarefaDto dto, int projetoId);

        Task AtualizarTarefaAsync(int id, AtualizarTarefaDto dto, int usuarioId);

        Task RemoverTarefaAsync(int id);
    }
}