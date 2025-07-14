using AutoMapper;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Application.DTOs.Projeto;
using ProjectManagement.Application.DTOs;

public class ProjetoProfile : Profile
{
    public ProjetoProfile()
    {
        CreateMap<AtualizarProjetoDto, Projeto>();
        CreateMap<CriarProjetoDto, Projeto>();
        CreateMap<Projeto, ProjetoDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProjetoID));

        CreateMap<AtualizarUsuarioDto, Usuario>();
        CreateMap<CriarUsuarioDto, Usuario>();
        CreateMap<Usuario, UsuarioDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UsuarioID));

        CreateMap<CriarTarefaDto, Tarefa>()
            .ForMember(dest => dest.TarefaID, opt => opt.Ignore());
        CreateMap<Tarefa, TarefaDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TarefaID));
    }
}
