using ProjectManagement.Application.DTOs;

namespace ProjectManagement.Domain.Interfaces.Services
{
    public interface IUsuarioService
    {

        Task<UsuarioDto> CriarUsuarioAsync(CriarUsuarioDto dto);

        Task<UsuarioDto> ObterUsuarioPorIdAsync(int id);

        Task<UsuarioDto> ObterUsuarioPorEmailAsync(string email);

        Task AtualizarUsuarioAsync(AtualizarUsuarioDto dto);

        Task DeletarUsuarioAsync(int id);

        Task<string> LoginAsync(string email, string senha);

    }
}
