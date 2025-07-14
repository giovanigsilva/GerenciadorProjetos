using FluentValidation;
using ProjectManagement.Application.DTOs;

public class CriarTarefaDtoValidator : AbstractValidator<CriarTarefaDto>
{
    public CriarTarefaDtoValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

        RuleFor(x => x.DataVencimento)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("A data de vencimento não pode ser no passado.")
            .When(x => x.DataVencimento.HasValue);

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("O status é obrigatório.")
            .Must(s => new[] { "pendente", "concluida", "em andamento" }.Contains(s.ToLower()))
            .WithMessage("Status inválido. Valores aceitos: pendente, em andamento, concluida.");

        RuleFor(x => x.Prioridade)
            .NotEmpty().WithMessage("A prioridade é obrigatória.")
            .Must(p => new[] { "baixa", "media", "alta" }.Contains(p.ToLower()))
            .WithMessage("Prioridade inválida. Valores aceitos: baixa, media, alta.");
    }
}
