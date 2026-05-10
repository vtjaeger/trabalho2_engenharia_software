using System.ComponentModel.DataAnnotations.Schema;

namespace trabalho2.Domain.Usuarios
{
    [Table("usuarios_logs")]
    public class UsuarioLog
    {
        public string Id { get; set; }
        public string UsuarioId { get; set; }
        public string Campo { get; set; }
        public string ValorAntigo { get; set; }
        public string UsuarioAlteracao { get; set; }
        public DateTime DataHora { get; set; }
    }
}
