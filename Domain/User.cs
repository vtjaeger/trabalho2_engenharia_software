namespace trabalho2.Domain
{
    public class User
    {
        public string Id { get; set; }
        public string Nome { get; set; } 
        public string Usuario { get; set; } 
        public string Senha { get; set; }
        public string Email { get; set; } 
        public UserRole Role { get; set; }  
        public DateTime DataCadastro { get; set; }
        public string Situacao { get; set; }    
    }
}
