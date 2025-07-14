using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities
{
    public class HistoricoTarefa
    {

        public int HistoricoID { get; set; }
        public int TarefaID { get; set; }
        public int UsuarioID { get; set; }
        public string? DescricaoAlteracao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public Tarefa? Tarefa { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
