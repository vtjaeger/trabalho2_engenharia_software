namespace trabalho2.Domain.Dtos.Request
{
    public class CreateUserRequest
    {
        public string Nome { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; } 
        public string Email { get; set; }
        //public UserRole Role { get; set; }
    }
}
