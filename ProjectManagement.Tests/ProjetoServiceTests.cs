using Moq;
using Xunit;
using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Services;
using ProjectManagement.Application.DTOs.Projeto;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces;
using ProjectManagement.Domain.Interfaces.Repositories;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Tests.Application.Services
{
    public class ProjetoServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ProjetoService>> _loggerMock;
        private readonly ProjetoService _service;
        private readonly Mock<IProjetoRepository> _projetoRepositoryMock;

        public ProjetoServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ProjetoService>>();
            _projetoRepositoryMock = new Mock<IProjetoRepository>();

            _unitOfWorkMock.Setup(u => u.Projetos)
                .Returns(_projetoRepositoryMock.Object);

            _service = new ProjetoService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CriarProjeto_DadosValidos_DeveSalvarProjeto()
        {
            var novoProjetoDto = new CriarProjetoDto
            {
                Nome = "Sistema de Projetos",
                Descricao = "Projeto completo inicial",
                UsuarioID = 1
            };

            var projetoEntity = new Projeto
            {
                Nome = novoProjetoDto.Nome,
                Descricao = novoProjetoDto.Descricao
            };

            _mapperMock.Setup(m => m.Map<Projeto>(novoProjetoDto))
                .Returns(projetoEntity);

            _projetoRepositoryMock.Setup(r => r.AddAsync(projetoEntity))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mapperMock.Setup(m => m.Map<ProjetoDto>(projetoEntity))
                .Returns(new ProjetoDto { Nome = projetoEntity.Nome });

            var resultado = await _service.CriarProjetoAsync(novoProjetoDto);

            Assert.NotNull(resultado);
            Assert.Equal("Sistema de Projetos", resultado.Nome);
            _projetoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Projeto>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CriarProjeto_SaveChangesFalha_DeveRetornarNull()
        {
            var projetoDto = new CriarProjetoDto { Nome = "Salvar Fail", UsuarioID = 2 };
            var projeto = new Projeto { Nome = "Salvar Fail" };

            _mapperMock.Setup(m => m.Map<Projeto>(projetoDto))
                .Returns(projeto);

            _projetoRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Projeto>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(0); // Falhou

            var resultado = await _service.CriarProjetoAsync(projetoDto);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task CriarProjeto_MapperRetornaNulo_DeveLancarExcecao()
        {
            // Arrange
            var projetoDto = new CriarProjetoDto { Nome = "Mapa ruim", UsuarioID = 3 };

            _mapperMock.Setup(m => m.Map<Projeto>(projetoDto))
                .Returns((Projeto)null); 

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CriarProjetoAsync(projetoDto)
            );
        }

        [Fact]
        public async Task CriarProjeto_AddAsyncLancaExcecao_DevePropagarErro()
        {
            var projetoDto = new CriarProjetoDto { Nome = "Erro Add", UsuarioID = 4 };
            var projeto = new Projeto { Nome = "Erro Add" };

            _mapperMock.Setup(m => m.Map<Projeto>(projetoDto))
                .Returns(projeto);

            _projetoRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Projeto>()))
                .ThrowsAsync(new Exception("Banco off"));

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.CriarProjetoAsync(projetoDto)
            );

            Assert.Equal("Banco off", ex.Message);
        }

        [Fact]
        public async Task CriarProjeto_DtoInvalido_DeveLancarValidationException()
        {
            var dtoInvalido = new CriarProjetoDto
            {
                Nome = "",
                Descricao = "Descrição inválida",
                UsuarioID = 0
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _service.CriarProjetoAsync(dtoInvalido)
            );
        }
    }
}
