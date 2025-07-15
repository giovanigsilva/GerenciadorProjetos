using ProjectManagement.Domain.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUsuarioRepository Usuarios { get; }
    IProjetoRepository Projetos { get; }
    ITarefaRepository Tarefas { get; }
    IHistoricoTarefaRepository HistoricoTarefas { get; }
    IComentarioRepository Comentarios { get; }  
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
