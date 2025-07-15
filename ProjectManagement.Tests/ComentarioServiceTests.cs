using Moq;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Interfaces.Repositories;

public class ComentarioServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IComentarioRepository> _comentarioRepositoryMock;
    private readonly ComentarioService _service;

    public ComentarioServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _comentarioRepositoryMock = new Mock<IComentarioRepository>();
        
        _uowMock.Setup(u => u.Comentarios).Returns(_comentarioRepositoryMock.Object);

        _service = new ComentarioService(_uowMock.Object);
    }

    [Fact]
    public async Task AdicionarComentarioViaStoredProcedureAsync_ChamaRepositorio_ComitaTransacao()
    {
        var dto = new AdicionarComentarioDto
        {
            UsuarioID = 1,
            TarefaID = 2,
            Comentario = "Comentário teste"
        };

        _comentarioRepositoryMock
            .Setup(repo => repo.AdicionarComentarioViaStoredProcedureAsync(dto))
            .Returns(Task.CompletedTask);

        _uowMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _service.AdicionarComentarioViaStoredProcedureAsync(dto);

        _comentarioRepositoryMock.Verify(
            repo => repo.AdicionarComentarioViaStoredProcedureAsync(dto),
            Times.Once);

        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AdicionarComentarioViaStoredProcedureAsync_NullDto_LancaArgumentNullException()
    {
        AdicionarComentarioDto dto = null;

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.AdicionarComentarioViaStoredProcedureAsync(dto));
    }
}
