namespace trabalho2.Domain
{
    public class UserLog
    {
        public string Id { get; set; }
        public string UsuarioId { get; set; }
        public string Campo { get; set; }
        public string ValorAntigo { get; set; }
        public string UsuarioAlteracao { get; set; }
        public DateTime DataHora { get; set; }
    }
}
