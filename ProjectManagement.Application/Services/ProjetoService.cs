using Microsoft.Extensions.Logging;
using ProjectManagement.Application.DTOs.Projeto;
using ProjectManagement.Domain.Entities;
using AutoMapper;
using ProjectManagement.Application.Exceptions;
using ProjectManagement.Domain.Interfaces.Services;

namespace ProjectManagement.Application.Services
{
    public class ProjetoService : IProjetoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProjetoService> _logger;

        public ProjetoService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProjetoService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjetoDto> CriarProjetoAsync(CriarProjetoDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Validação: usuário existe?
            var usuario = await _unitOfWork.Usuarios.ObterPorIdAsync(dto.UsuarioID);
            if (usuario == null)
                throw new InvalidOperationException("Usuário não encontrado.");

            var projeto = _mapper.Map<Projeto>(dto);

            await _unitOfWork.Projetos.AddAsync(projeto);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Projeto criado com sucesso: {Nome}", projeto.Nome);

            return _mapper.Map<ProjetoDto>(projeto);
        }

        public async Task<ProjetoDto> ObterPorIdAsync(int id)
        {
            var projeto = await _unitOfWork.Projetos.GetByIdAsync(id);
            if (projeto == null)
                throw new NotFoundException("Projeto não encontrado.");

            return _mapper.Map<ProjetoDto>(projeto);
        }

        public async Task<IEnumerable<ProjetoDto>> GetAllByUsuarioAsync(int usuarioId)
        {
            var projetos = await _unitOfWork.Projetos.GetAllByUsuarioAsync(usuarioId);
            return _mapper.Map<IEnumerable<ProjetoDto>>(projetos);
        }

        public async Task AtualizarProjetoAsync(AtualizarProjetoDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var projeto = await _unitOfWork.Projetos.GetByIdAsync(dto.Id);
            if (projeto == null)
                throw new NotFoundException("Projeto não encontrado.");

            _mapper.Map(dto, projeto);
            _unitOfWork.Projetos.Update(projeto);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeletarProjetoAsync(int id)
        {
            var temTarefasPendentes = await _unitOfWork.Projetos.TemTarefasPendentesAsync(id);
            if (temTarefasPendentes)
                throw new InvalidOperationException(
                    "Não é permitido excluir projeto que possua tarefas pendentes. Finalize ou exclua as tarefas antes."
                );

            var projeto = await _unitOfWork.Projetos.GetByIdAsync(id);
            if (projeto == null)
                throw new NotFoundException("Projeto não encontrado.");

            _unitOfWork.Projetos.Remove(projeto);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
