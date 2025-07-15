using ProjectManagement.Application.DTOs;

public interface IComentarioService
{
    Task AdicionarComentarioViaStoredProcedureAsync(AdicionarComentarioDto dto);
    Task AdicionarComentarioDiretoAsync(AdicionarComentarioDto dto);
}
