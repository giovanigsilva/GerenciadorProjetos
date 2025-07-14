using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces.Repositories;
using ProjectManagement.Domain.Interfaces.Services;
using ProjectManagement.Infrastructure.Data;
using ProjectManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    private IUsuarioRepository _usuarios;
    private IProjetoRepository _projetos;
    private ITarefaRepository _tarefas;
    private IHistoricoTarefaRepository _historicoTarefas;

    public IUsuarioRepository Usuarios =>
        _usuarios ??= new UsuarioRepository(_context);

    public IProjetoRepository Projetos =>
        _projetos ??= new ProjetoRepository(_context);

    public ITarefaRepository Tarefas =>
        _tarefas ??= new TarefaRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
    public IHistoricoTarefaRepository HistoricoTarefas =>
        _historicoTarefas ??= new HistoricoTarefaRepository(_context);
    public void Dispose() => _context.Dispose();
    public async ValueTask DisposeAsync() => await _context.DisposeAsync();
}