using System.ComponentModel.DataAnnotations.Schema;

namespace trabalho2.Domain.Usuarios
{
    [Table("usuarios")]
    public class User
    {
        public string Id { get; set; }
        public string Nome { get; set; } 
        public string Usuario { get; set; } 
        public string Senha { get; set; }
        public string Email { get; set; } 
        public UserRole Role { get; set; }

        [Column("data_cadastro")]
        public DateTime DataCadastro { get; set; }
        public string Situacao { get; set; }    
    }
}
