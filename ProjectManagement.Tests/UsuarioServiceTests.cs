using Moq;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using ProjectManagement.Application.Services;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces.Repositories;

namespace ProjectManagement.Tests.Application.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _configurationMock = new Mock<IConfiguration>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();

            _unitOfWorkMock.Setup(u => u.Usuarios)
                .Returns(_usuarioRepositoryMock.Object);

            var jwtSection = new Mock<IConfigurationSection>();
            jwtSection.Setup(x => x.Value).Returns("segredo_jwt_mocado_padrao");
            _configurationMock.Setup(c => c[It.IsAny<string>()])
                .Returns("segredo_jwt_mocado_padrao");
            _configurationMock.Setup(c => c.GetSection(It.IsAny<string>()))
                .Returns(jwtSection.Object);

            _service = new UsuarioService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _configurationMock.Object
            );
        }

        [Fact]
        public async Task CriarUsuarioAsync_DadosValidos_DeveSalvarEMapear()
        {
            // Arrange
            var criarDto = new CriarUsuarioDto { Email = "user@teste.co.brm", Senha = "123456" };
            var usuarioNovo = new Usuario { UsuarioID = 1, Email = criarDto.Email, SenhaHash = "HASH" };
            var usuarioDto = new UsuarioDto { UsuarioID = 1, Email = criarDto.Email };

            _usuarioRepositoryMock.Setup(r => r.ObterPorEmailAsync(criarDto.Email))
                .ReturnsAsync((Usuario)null);
            _mapperMock.Setup(m => m.Map<Usuario>(criarDto))
                .Returns(usuarioNovo);
            _usuarioRepositoryMock.Setup(r => r.AdicionarAsync(usuarioNovo))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<UsuarioDto>(usuarioNovo))
                .Returns(usuarioDto);

            var result = await _service.CriarUsuarioAsync(criarDto);

            Assert.NotNull(result);
            Assert.Equal(criarDto.Email, result.Email);
            _usuarioRepositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task CriarUsuarioAsync_EmailExistente_DeveLancarInvalidOperationException()
        {
            var criarDto = new CriarUsuarioDto { Email = "ja@existe.com.br", Senha = "123" };
            _usuarioRepositoryMock.Setup(r => r.ObterPorEmailAsync(criarDto.Email))
                .ReturnsAsync(new Usuario { Email = criarDto.Email });

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CriarUsuarioAsync(criarDto));
        }

        [Fact]
        public async Task ObterUsuarioPorIdAsync_UsuarioExiste_RetornaUsuario()
        {
            var usuario = new Usuario { UsuarioID = 5, Email = "a@b.com" };
            var usuarioDto = new UsuarioDto { UsuarioID = 5, Email = "a@b.com" };
            _usuarioRepositoryMock.Setup(r => r.ObterPorIdAsync(5))
                .ReturnsAsync(usuario);
            _mapperMock.Setup(m => m.Map<UsuarioDto>(usuario)).Returns(usuarioDto);

            var result = await _service.ObterUsuarioPorIdAsync(5);

            Assert.NotNull(result);
            Assert.Equal(5, result.UsuarioID);
        }

        [Fact]
        public async Task ObterUsuarioPorIdAsync_NaoExiste_DeveLancarKeyNotFoundException()
        {
            _usuarioRepositoryMock.Setup(r => r.ObterPorIdAsync(7))
                .ReturnsAsync((Usuario)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.ObterUsuarioPorIdAsync(7));
        }

        [Fact]
        public async Task AtualizarUsuarioAsync_UsuarioNaoExiste_DeveLancarExcecao()
        {
            var atualizarDto = new AtualizarUsuarioDto { Id = 99, Email = "novo@x.com" };
            _usuarioRepositoryMock.Setup(r => r.ObterPorIdAsync(99))
                .ReturnsAsync((Usuario)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.AtualizarUsuarioAsync(atualizarDto));
        }

        [Fact]
        public async Task DeletarUsuarioAsync_UsuarioNaoExiste_DeveLancarExcecao()
        {
            _usuarioRepositoryMock.Setup(r => r.ObterPorIdAsync(1234))
                .ReturnsAsync((Usuario)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.DeletarUsuarioAsync(1234));
        }

        [Fact]
        public async Task LoginAsync_CredenciaisCorretas_RetornaToken()
        {
            string email = "login@eu.com.br";
            string senha = "senha123";
            string hashCorreto = BCrypt.Net.BCrypt.HashPassword(senha);
            var usuario = new Usuario { UsuarioID = 2, Email = email, SenhaHash = hashCorreto, Funcao = "user" };

            _usuarioRepositoryMock.Setup(r => r.ObterPorEmailAsync(email))
                .ReturnsAsync(usuario);

            var token = await _service.LoginAsync(email, senha);

            Assert.NotNull(token);
            Assert.True(token.Length > 10); // JWT típico
        }

        [Fact]
        public async Task LoginAsync_UsuarioNaoExiste_DeveLancarUnauthorizedAccess()
        {
            _usuarioRepositoryMock.Setup(r => r.ObterPorEmailAsync("semUser@x.com"))
                .ReturnsAsync((Usuario)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.LoginAsync("semUser@x.com", "12345"));
        }

        [Fact]
        public async Task LoginAsync_SenhaIncorreta_DeveLancarUnauthorizedAccess()
        {
            var user = new Usuario
            {
                UsuarioID = 11,
                Email = "fulano@teste.com.br",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("certa"),
                Funcao = "user"
            };
            _usuarioRepositoryMock.Setup(r => r.ObterPorEmailAsync(user.Email))
                .ReturnsAsync(user);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.LoginAsync(user.Email, "errada"));
        }
    }
}
