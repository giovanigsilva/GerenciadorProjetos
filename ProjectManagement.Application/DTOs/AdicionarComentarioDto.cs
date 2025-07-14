using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Application.DTOs
{

    public class AdicionarComentarioDto
    {
        [Required(ErrorMessage = "O ID da tarefa é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Informe um ID de tarefa válido.")]
        public int TarefaID { get; set; }

        [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Informe um ID de usuário válido.")]
        public int UsuarioID { get; set; }

        [StringLength(1000, ErrorMessage = "O comentário não pode exceder 1000 caracteres.")]
        public string? Comentario { get; set; }
    }

}
