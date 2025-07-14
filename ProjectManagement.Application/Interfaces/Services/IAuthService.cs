using ProjectManagement.Application.DTOs;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string senha);
        Task<UsuarioDto> RegistrarUsuarioAsync(CriarUsuarioDto dto);
        bool ValidarToken(string token);
        Task AlterarSenhaAsync(int usuarioId, string senhaAtual, string novaSenha);
        Task<string> GerarTokenRecuperacaoSenhaAsync(string email);
        Task RedefinirSenhaAsync(string token, string novaSenha);
        Task<string> HashPassword(string password);
    }
}