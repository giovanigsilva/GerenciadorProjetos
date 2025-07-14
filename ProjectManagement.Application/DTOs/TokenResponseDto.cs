using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.DTOs
{
    public class TokenResponseDto
    {
        [Required(ErrorMessage = "O token é obrigatório.")]
        public string? Token { get; set; }

        [Required(ErrorMessage = "A data de expiração é obrigatória.")]
        [DataType(DataType.DateTime, ErrorMessage = "Data de expiração inválida.")]
        public DateTime ExpiraEm { get; set; }

        [Required(ErrorMessage = "O e-mail do usuário é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail do usuário não é válido.")]
        [StringLength(254, ErrorMessage = "O e-mail do usuário não pode exceder 254 caracteres.")]
        public string? UsuarioEmail { get; set; }
    }
}
