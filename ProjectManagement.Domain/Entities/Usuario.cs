using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Domain.Entities
{
    public class Usuario
{
    public int UsuarioID { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? TokenRecuperacaoSenha { get; set; }
    public DateTime? DataExpiracaoToken { get; set; }
    public string? SenhaHash { get; set; }
    public string? Funcao { get; set; }
    public DateTime DataCriacao { get; set; }
    public ICollection<Projeto> Projetos { get; } = new List<Projeto>();
}
}
