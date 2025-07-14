using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities
{
    public class Projeto
{
    public int ProjetoID { get; set; }
    public int UsuarioID { get; set; }
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public DateTime DataCriacao { get; set; }
    public Usuario? Usuario { get; set; }
    public ICollection<Tarefa> Tarefas { get; } = new List<Tarefa>();
}
}
