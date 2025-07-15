using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Entities;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Interfaces.Repositories
{
    public interface IComentarioRepository
    {
        Task AdicionarComentarioViaStoredProcedureAsync(AdicionarComentarioDto dto);

    }
}
