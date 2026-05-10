using Microsoft.EntityFrameworkCore;
using trabalho2.Domain.Tarefas;
using trabalho2.Domain.Usuarios;

namespace trabalho2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) // pra salvar a descricao do enum e não número
        { 
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Tarefa>()
                .Property(x => x.Situacao)
                .HasConversion<string>();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UsuarioLog> UserLogs { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }

    }
}