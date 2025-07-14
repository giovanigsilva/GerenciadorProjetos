using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.DTOs.Projeto
{
    public class AtualizarProjetoDto
    {
        [Required(ErrorMessage = "O ID do projeto é obrigatório.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do projeto é obrigatório.")]
        [StringLength(150, ErrorMessage = "O nome não pode exceder 150 caracteres.")]
        public string? Nome { get; set; }

        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
        public string? Descricao { get; set; }
    }
}