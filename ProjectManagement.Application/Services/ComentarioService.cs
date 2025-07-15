using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Entities;
using System;

public class ComentarioService : IComentarioService
{
    private readonly IUnitOfWork _unitOfWork;

    public ComentarioService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AdicionarComentarioViaStoredProcedureAsync(AdicionarComentarioDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        await _unitOfWork.Comentarios.AdicionarComentarioViaStoredProcedureAsync(dto);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AdicionarComentarioDiretoAsync(AdicionarComentarioDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var historico = new HistoricoTarefa
        {
            TarefaID = dto.TarefaID,
            UsuarioID = dto.UsuarioID,
            DescricaoAlteracao = $"Comentário adicionado: {dto.Comentario}",
            DataAlteracao = DateTime.UtcNow
        };

        _unitOfWork.HistoricoTarefas.AdicionarAsync(historico);
        await _unitOfWork.SaveChangesAsync();
    }
}
