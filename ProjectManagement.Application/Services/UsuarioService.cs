using ProjectManagement.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using ProjectManagement.Application.Exceptions;
using Microsoft.Extensions.Configuration;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Interfaces.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProjectManagement.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UsuarioService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UsuarioDto> CriarUsuarioAsync(CriarUsuarioDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var usuarioExistente = await _unitOfWork.Usuarios.ObterPorEmailAsync(dto.Email);
            if (usuarioExistente != null)
                throw new InvalidOperationException("E-mail já cadastrado.");

            var usuario = _mapper.Map<Usuario>(dto);
            usuario.SenhaHash = HashPassword(dto.Senha);

            await _unitOfWork.Usuarios.AdicionarAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UsuarioDto>(usuario);
        }

        public async Task<UsuarioDto> ObterUsuarioPorIdAsync(int id)
        {
            var usuario = await _unitOfWork.Usuarios.ObterPorIdAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            return _mapper.Map<UsuarioDto>(usuario);
        }

        public async Task<UsuarioDto> ObterUsuarioPorEmailAsync(string email)
        {
            var usuario = await _unitOfWork.Usuarios.ObterPorEmailAsync(email);
            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            return _mapper.Map<UsuarioDto>(usuario);
        }

        public async Task AtualizarUsuarioAsync(AtualizarUsuarioDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var usuario = await _unitOfWork.Usuarios.ObterPorIdAsync(dto.Id);
            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            _mapper.Map(dto, usuario);
            _unitOfWork.Usuarios.Atualizar(usuario);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeletarUsuarioAsync(int id)
        {
            var usuario = await _unitOfWork.Usuarios.ObterPorIdAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            _unitOfWork.Usuarios.Remover(usuario);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<string> LoginAsync(string email, string senha)
        {
            var usuario = await _unitOfWork.Usuarios.ObterPorEmailAsync(email);
            if (usuario == null || !VerifyPassword(senha, usuario.SenhaHash))
                throw new UnauthorizedAccessException("Credenciais inválidas.");

            return GenerateJwtToken(usuario);
        }

        #region Métodos Auxiliares

        private string HashPassword(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        private bool VerifyPassword(string password, string hashedPassword) =>
            BCrypt.Net.BCrypt.Verify(password, hashedPassword);

        private string GenerateJwtToken(Usuario usuario)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
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
