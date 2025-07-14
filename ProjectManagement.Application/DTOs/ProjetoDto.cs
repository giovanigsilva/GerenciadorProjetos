using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.DTOs.Projeto
{
    public class ProjetoDto
    {
        [Required(ErrorMessage = "O Id do projeto é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O Id do projeto deve ser um número positivo.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do projeto é obrigatório.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O nome deve conter entre 3 e 200 caracteres.")]
        public string? Nome { get; set; }

        [StringLength(1000, ErrorMessage = "A descrição não pode exceder 1000 caracteres.")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O Id do usuário é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O Id do usuário deve ser um número positivo.")]
        public int UsuarioID { get; set; }

        [Required(ErrorMessage = "A data de criação é obrigatória.")]
        [DataType(DataType.DateTime, ErrorMessage = "Data de criação inválida.")]
        public DateTime DataCriacao { get; set; }
    }
}
