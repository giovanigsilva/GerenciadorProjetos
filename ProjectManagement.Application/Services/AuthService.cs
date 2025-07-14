using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Interfaces.Services;
using ProjectManagement.Domain.Interfaces.Repositories;

namespace ProjectManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(string email, string senha)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
            if (usuario == null || !VerificarSenha(senha, usuario.SenhaHash))
                throw new UnauthorizedAccessException("Credenciais inválidas.");

            return GerarJwtToken(usuario);
        }
        
        public async Task<UsuarioDto> RegistrarUsuarioAsync(CriarUsuarioDto dto)
        {
            var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(dto.Email);
            if (usuarioExistente != null)
                throw new ArgumentException("E-mail já cadastrado.");

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = await HashPassword(dto.Senha),
                Funcao = dto.Funcao ?? "usuario",
                DataCriacao = DateTime.UtcNow
            };

            await _usuarioRepository.AdicionarAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            return new UsuarioDto
            {
                Id = usuario.UsuarioID,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Funcao = usuario.Funcao,
                DataCriacao = usuario.DataCriacao
            };
        }

        public bool ValidarToken(string token)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"])),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, validationParameters, out var validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task AlterarSenhaAsync(int usuarioId, string senhaAtual, string novaSenha)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            if (!VerificarSenha(senhaAtual, usuario.SenhaHash))
                throw new ArgumentException("Senha atual incorreta.");

            usuario.SenhaHash = await HashPassword(novaSenha);
            _usuarioRepository.Atualizar(usuario);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<string> GerarTokenRecuperacaoSenhaAsync(string email)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            var token = Guid.NewGuid().ToString();
            usuario.TokenRecuperacaoSenha = await HashPassword(token);
            usuario.DataExpiracaoToken = DateTime.UtcNow.AddHours(1);

            _usuarioRepository.Atualizar(usuario);
            await _unitOfWork.SaveChangesAsync();

            return token;
        }

        public async Task RedefinirSenhaAsync(string token, string novaSenha)
        {
            var usuario = await _usuarioRepository.ObterPorHashDoTokenDeRecuperacaoAsync(token);
            if (usuario == null || usuario.DataExpiracaoToken < DateTime.UtcNow)
                throw new ArgumentException("Token inválido ou expirado.");

            usuario.SenhaHash = await HashPassword(novaSenha);
            usuario.TokenRecuperacaoSenha = null;
            usuario.DataExpiracaoToken = null;

            _usuarioRepository.Atualizar(usuario);
            await _unitOfWork.SaveChangesAsync();
        }

        #region Métodos Privados

        public async Task<string> HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerificarSenha(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private string GerarJwtToken(Usuario usuario)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Funcao)
            };

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }


        #endregion
    }
}