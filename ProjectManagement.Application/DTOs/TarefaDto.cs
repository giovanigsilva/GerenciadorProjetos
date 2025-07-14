using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.DTOs
{
    public class TarefaDto
    {
        [Required(ErrorMessage = "O ID da tarefa é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O ID da tarefa deve ser um número positivo.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O título deve conter entre 3 e 200 caracteres.")]
        public string? Titulo { get; set; }

        [StringLength(1000, ErrorMessage = "A descrição não pode exceder 1000 caracteres.")]
        public string? Descricao { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Data de vencimento inválida.")]
        public DateTime? DataVencimento { get; set; }

        [StringLength(50, ErrorMessage = "O status não pode exceder 50 caracteres.")]
        public string? Status { get; set; } 

        [StringLength(50, ErrorMessage = "A prioridade não pode exceder 50 caracteres.")]
        public string? Prioridade { get; set; }

        [Required(ErrorMessage = "A data de criação é obrigatória.")]
        [DataType(DataType.DateTime, ErrorMessage = "Data de criação inválida.")]
        public DateTime DataCriacao { get; set; }
    }
}
