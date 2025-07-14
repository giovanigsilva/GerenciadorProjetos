using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.DTOs.Projeto
{
    using System.ComponentModel.DataAnnotations;

    public class CriarProjetoDto
    {
        [Required(ErrorMessage = "O nome do projeto é obrigatório.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "O nome do projeto deve ter entre 3 e 150 caracteres.")]
        public string Nome { get; set; }

        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Informe um ID de usuário válido.")]
        public int UsuarioID { get; set; }
    }

}