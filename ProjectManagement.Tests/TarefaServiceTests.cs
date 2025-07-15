using Moq;
using AutoMapper;
using ProjectManagement.Application.Services;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces.Repositories;
using ProjectManagement.Domain.Interfaces.Services;

namespace ProjectManagement.Tests.Application.Services
{
    public class TarefaServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITarefaRepository> _tarefaRepositoryMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly Mock<IHistoricoTarefaRepository> _historicoRepositoryMock;
        private readonly TarefaService _service;

        public TarefaServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _mapperMock = new Mock<IMapper>();
            _tarefaRepositoryMock = new Mock<ITarefaRepository>();
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            _historicoRepositoryMock = new Mock<IHistoricoTarefaRepository>();

            _unitOfWorkMock.Setup(u => u.Tarefas).Returns(_tarefaRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.Usuarios).Returns(_usuarioRepositoryMock.Object);
            _unitOfWorkMock.Setup(u => u.HistoricoTarefas).Returns(_historicoRepositoryMock.Object);

            _service = new TarefaService(
                _unitOfWorkMock.Object,
                _usuarioServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task ObterTodasPorProjetoAsync_DeveRetornarTarefasDto()
        {
            var projetoId = 1;
            var tarefas = new List<Tarefa> {
                new Tarefa { TarefaID = 1, Titulo = "Tarefa 1" },
                new Tarefa { TarefaID = 2, Titulo = "Tarefa 2" }
            };

            _tarefaRepositoryMock.Setup(r => r.ObterTodasPorProjetoAsync(projetoId))
                                 .ReturnsAsync(tarefas);
            _mapperMock.Setup(m => m.Map<IEnumerable<TarefaDto>>(tarefas))
                       .Returns(new List<TarefaDto> { new TarefaDto(), new TarefaDto() });

            var result = await _service.ObterTodasPorProjetoAsync(projetoId);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AdicionarTarefaAsync_UsuarioInvalido_DeveLancarArgumentException()
        {
            var dto = new CriarTarefaDto { UsuarioID = 99, Status = "pendente", Prioridade = "baixa" };
            _usuarioRepositoryMock.Setup(r => r.ObterPorIdAsync(dto.UsuarioID))
                                  .ReturnsAsync((Usuario)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AdicionarTarefaAsync(dto, 1));
        }

        [Fact]
        public async Task AdicionarTarefaAsync_StatusInvalido_DeveLancarArgumentException()
        {
            var dto = new CriarTarefaDto { UsuarioID = 1, Status = "inexistente", Prioridade = "alta" };
            _usuarioRepositoryMock.Setup(r => r.ObterPorIdAsync(dto.UsuarioID))
                .ReturnsAsync(new Usuario());

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AdicionarTarefaAsync(dto, 1));
        }

        [Fact]
        public async Task AdicionarTarefaAsync_QtdTarefasExcedido_DeveLancarInvalidOperationException()
        {
            var dto = new CriarTarefaDto { UsuarioID = 1, Status = "pendente", Prioridade = "alta" };

            _usuarioRepositoryMock.Setup(r => r.ObterPorIdAsync(dto.UsuarioID))
                                  .ReturnsAsync(new Usuario());

            _tarefaRepositoryMock.Setup(r => r.ObterTodasPorProjetoAsync(It.IsAny<int>()))
                                 .ReturnsAsync(Enumerable.Range(1, 20).Select(x => new Tarefa()).ToList());

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AdicionarTarefaAsync(dto, 1));
        }

        [Fact]
        public async Task AtualizarTarefaAsync_PrioridadeAlterada_DeveLancarInvalidOperationException()
        {
            var tarefa = new Tarefa { TarefaID = 1, Prioridade = "alta" };
            var dto = new AtualizarTarefaDto
            {
                Titulo = "Teste",
                Descricao = "Teste",
                Status = "pendente",
                Prioridade = "baixa",
                DataVencimento = DateTime.Now
            };

            _tarefaRepositoryMock.Setup(r => r.ObterPorIdAsync(tarefa.TarefaID))
                                 .ReturnsAsync(tarefa);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AtualizarTarefaAsync(tarefa.TarefaID, dto, 1));
        }

        [Fact]
        public async Task AtualizarTarefaAsync_DadosValidos_DeveAtualizarComHistorico()
        {
            var tarefa = new Tarefa
            {
                TarefaID = 1,
                Titulo = "Antigo",
                Status = "pendente",
                Descricao = "desc x",
                Prioridade = "baixa",
                DataVencimento = DateTime.Today
            };

            var dto = new AtualizarTarefaDto
            {
                Titulo = "Novo",
                Status = "concluida",
                Descricao = "desc y", 
                Prioridade = "baixa",
                DataVencimento = DateTime.Today.AddDays(1)
            };

            _tarefaRepositoryMock.Setup(t => t.ObterPorIdAsync(tarefa.TarefaID))
                                 .ReturnsAsync(tarefa);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(1);

            // Act
            await _service.AtualizarTarefaAsync(tarefa.TarefaID, dto, usuarioId: 1);

            // Assert
            _tarefaRepositoryMock.Verify(t => t.Atualizar(It.IsAny<Tarefa>()), Times.Once);
            _historicoRepositoryMock.Verify(h => h.AdicionarAsync(It.IsAny<HistoricoTarefa>()), Times.Exactly(3));
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoverTarefaAsync_TarefaNaoExiste_DeveLancarKeyNotFoundException()
        {
            _tarefaRepositoryMock.Setup(t => t.ObterPorIdAsync(99))
                                 .ReturnsAsync((Tarefa)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.RemoverTarefaAsync(99));
        }

        [Fact]
        public async Task RemoverTarefaAsync_TarefaValida_DeveRemover()
        {
            var tarefa = new Tarefa { TarefaID = 1 };

            _tarefaRepositoryMock.Setup(t => t.ObterPorIdAsync(tarefa.TarefaID))
                                 .ReturnsAsync(tarefa);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(1);

            await _service.RemoverTarefaAsync(tarefa.TarefaID);

            _tarefaRepositoryMock.Verify(r => r.Remover(tarefa), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
