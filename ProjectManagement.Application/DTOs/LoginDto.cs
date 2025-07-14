using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        [StringLength(254, ErrorMessage = "O e-mail não pode exceder 254 caracteres.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve conter entre 6 e 100 caracteres.")]
        public string? Senha { get; set; }
    }
}
