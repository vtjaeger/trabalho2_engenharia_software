using System.ComponentModel.DataAnnotations.Schema;

namespace trabalho2.Domain.Tarefas
{
    [Table("tarefas")]
    public class Tarefa
    {
        public string Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public TarefaSituacaoEnum Situacao { get; set; }
        public string Usuario { get; set; }

        [Column("inicio_data_hora")]
        public DateTime InicioDataHora { get; set; }

        [Column("fim_data_hora")]
        public DateTime FimDataHora { get; set; }
    }
}
