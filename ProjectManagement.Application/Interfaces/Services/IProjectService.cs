// Deve estar em Application/Interfaces
using ProjectManagement.Application.DTOs.Projeto;

public interface IProjetoService
{
    Task<ProjetoDto> CriarProjetoAsync(CriarProjetoDto dto);
    Task<IEnumerable<ProjetoDto>> GetAllByUsuarioAsync(int usuarioId);
    Task<ProjetoDto> ObterPorIdAsync(int id);
    Task AtualizarProjetoAsync(AtualizarProjetoDto dto);
    Task DeletarProjetoAsync(int id);
}