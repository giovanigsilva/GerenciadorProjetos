using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities
{
    public class Tarefa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TarefaID { get; set; }
        public int ProjetoID { get; set; }
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public DateTime? DataVencimento { get; set; }
        public string? Status { get; set; }
        public string? Prioridade { get; set; }
        public DateTime DataCriacao { get; set; }
        public Projeto? Projeto { get; set; }
        public ICollection<HistoricoTarefa> Historico { get; } = new List<HistoricoTarefa>();
        public int UsuarioID { get; set; }
    }
}
