using System.Threading.Tasks;

namespace ProjectManagement.Domain.Interfaces.Services
{
    public interface IRelatorioService
    {
        Task<double> RelatorioDesempenhoAsync(int usuarioLogadoId);
    }
}