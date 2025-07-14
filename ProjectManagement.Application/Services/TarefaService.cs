using AutoMapper;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Enums;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Interfaces.Services;

namespace ProjectManagement.Application.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuarioService _usuarioService;
        private readonly IMapper _mapper;

        public TarefaService(IUnitOfWork unitOfWork, IUsuarioService usuarioService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _usuarioService = usuarioService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TarefaDto>> ObterTodasPorProjetoAsync(int projetoId)
        {
            var tarefas = await _unitOfWork.Tarefas.ObterTodasPorProjetoAsync(projetoId);
            return _mapper.Map<IEnumerable<TarefaDto>>(tarefas);
        }

        public async Task AdicionarTarefaAsync(CriarTarefaDto dto, int projetoId)
        {
            var usuario = await _unitOfWork.Usuarios.ObterPorIdAsync(dto.UsuarioID);
if (usuario == null)
    throw new ArgumentException("Usuário responsável pela tarefa não existe.");

            if (!Enum.TryParse<StatusTarefaEnum>(dto.Status, true, out _))
                throw new ArgumentException("Status inválido. Os valores aceitos são: pendente, concluida.");

            if (!Enum.TryParse<PrioridadeTarefaEnum>(dto.Prioridade, true, out _))
                throw new ArgumentException("Prioridade inválida. Os valores aceitos são: baixa, media, alta.");

            var tarefasProjeto = await _unitOfWork.Tarefas.ObterTodasPorProjetoAsync(projetoId);
            if (tarefasProjeto.Count() >= 20)
                throw new InvalidOperationException("O projeto já atingiu o limite máximo de 20 tarefas.");

            if (dto.Prioridade == null)
                throw new InvalidOperationException("A prioridade da tarefa é obrigatória.");

            var tarefa = _mapper.Map<Tarefa>(dto);
            tarefa.ProjetoID = projetoId;

            await _unitOfWork.Tarefas.AdicionarAsync(tarefa);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AtualizarTarefaAsync(int id, AtualizarTarefaDto dto, int usuarioId)
        {
            var tarefa = await _unitOfWork.Tarefas.ObterPorIdAsync(id);
            if (tarefa == null)
                throw new KeyNotFoundException("Tarefa não encontrada.");

            if (dto.Prioridade != tarefa.Prioridade)
                throw new InvalidOperationException("Não é permitido alterar a prioridade da tarefa.");

            var historicos = new List<HistoricoTarefa>();
            if (tarefa.Status != dto.Status)
                historicos.Add(new HistoricoTarefa
                {
                    TarefaID = tarefa.TarefaID,
                    UsuarioID = usuarioId,
                    DescricaoAlteracao = $"Status alterado de '{tarefa.Status}' para '{dto.Status}'",
                    DataAlteracao = DateTime.UtcNow
                });
            if (tarefa.Titulo != dto.Titulo)
                historicos.Add(new HistoricoTarefa
                {
                    TarefaID = tarefa.TarefaID,
                    UsuarioID = usuarioId,
                    DescricaoAlteracao = $"Título alterado de '{tarefa.Titulo}' para '{dto.Titulo}'",
                    DataAlteracao = DateTime.UtcNow
                });
            if (tarefa.Descricao != dto.Descricao)
                historicos.Add(new HistoricoTarefa
                {
                    TarefaID = tarefa.TarefaID,
                    UsuarioID = usuarioId,
                    DescricaoAlteracao = $"Descrição alterada.",
                    DataAlteracao = DateTime.UtcNow
                });
            if (tarefa.DataVencimento != dto.DataVencimento)
                historicos.Add(new HistoricoTarefa
                {
                    TarefaID = tarefa.TarefaID,
                    UsuarioID = usuarioId,
                    DescricaoAlteracao = $"Data de vencimento alterada de '{tarefa.DataVencimento}' para '{dto.DataVencimento}'",
                    DataAlteracao = DateTime.UtcNow
                });

            tarefa.Status = dto.Status;
            tarefa.Titulo = dto.Titulo;
            tarefa.Descricao = dto.Descricao;
            tarefa.DataVencimento = dto.DataVencimento;

            _unitOfWork.Tarefas.Atualizar(tarefa);

            if (historicos.Any())
            {
                foreach (var hist in historicos)
                    await _unitOfWork.HistoricoTarefas.AdicionarAsync(hist);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoverTarefaAsync(int id)
        {
            var tarefa = await _unitOfWork.Tarefas.ObterPorIdAsync(id);
            if (tarefa == null)
                throw new KeyNotFoundException("Tarefa não encontrada.");

            _unitOfWork.Tarefas.Remover(tarefa);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
