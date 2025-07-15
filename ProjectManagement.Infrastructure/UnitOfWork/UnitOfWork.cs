using ProjectManagement.Domain.Interfaces.Repositories;
using ProjectManagement.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IUsuarioRepository Usuarios { get; }
    public IProjetoRepository Projetos { get; }
    public ITarefaRepository Tarefas { get; }
    public IHistoricoTarefaRepository HistoricoTarefas { get; }
    public IComentarioRepository Comentarios { get; } // <---

    public UnitOfWork(
        AppDbContext context,
        IUsuarioRepository usuarios,
        IProjetoRepository projetos,
        ITarefaRepository tarefas,
        IHistoricoTarefaRepository historicoTarefas,
        IComentarioRepository comentarios // <---
        )
    {
        _context = context;
        Usuarios = usuarios;
        Projetos = projetos;
        Tarefas = tarefas;
        HistoricoTarefas = historicoTarefas;
        Comentarios = comentarios; // <---
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    public void Dispose() => _context.Dispose();
}
