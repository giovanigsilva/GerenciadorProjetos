using ProjectManagement.Domain.Interfaces.Repositories;
using ProjectManagement.Domain.Interfaces.Services;

public class RelatorioService : IRelatorioService
{
    private readonly IUnitOfWork _unitOfWork;

    public RelatorioService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<double> RelatorioDesempenhoAsync(int usuarioLogadoId)
    {
        var usuario = await _unitOfWork.Usuarios.ObterPorIdAsync(usuarioLogadoId);
        if (usuario == null || !usuario.Funcao.Equals("gerente", StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Acesso restrito a gerentes.");

        var tarefas = await _unitOfWork.Tarefas.ObterConcluidasUltimos30DiasAsync();

        double media = tarefas.Any() ? (double)tarefas.Count() / 30 : 0;

        return media;
    }

}
