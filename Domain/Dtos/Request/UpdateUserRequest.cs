using System.ComponentModel.DataAnnotations;

namespace trabalho2.Domain.Dtos.Request
{
    public class UpdateUserRequest
    {
        public string? Usuario { get; set; } 
        public string? Email { get; set; }
        public UserRole? Role { get; set; }
    }
}
