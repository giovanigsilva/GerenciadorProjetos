using System;
using System.ComponentModel.DataAnnotations;

public class AtualizarTarefaDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "O título deve conter entre 3 e 200 caracteres.")]
    public string Titulo { get; set; } = null!; 

    [StringLength(1000, ErrorMessage = "A descrição não pode exceder 1000 caracteres.")]
    public string? Descricao { get; set; }

    public DateTime? DataVencimento { get; set; }

    [StringLength(50, ErrorMessage = "O status não pode exceder 50 caracteres.")]
    [RegularExpression("pendente|em andamento|concluída", ErrorMessage = "Status inválido. Use: pendente, em andamento ou concluída.")]
    public string? Status { get; set; }
    public string? Prioridade { get; set; }
}
