using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.DTOs
{
    public class AtualizarUsuarioDto
    {
        [Required(ErrorMessage = "O Id do usuário é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O Id do usuário deve ser um número positivo.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "O nome deve conter entre 3 e 150 caracteres.")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        [StringLength(254, ErrorMessage = "O e-mail não pode exceder 254 caracteres.")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "A função não pode exceder 100 caracteres.")]
        public string? Funcao { get; set; }
    }
}
