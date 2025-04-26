using System.ComponentModel.DataAnnotations.Schema;

namespace LogisControlAPI.Models
{
    public class NotaEncomendaItens
    {
        public int NotaEncomendaItensId { get; set; }
        public int NotaEncomendaId { get; set; }             // ← Chave FK
        public virtual NotaEncomenda NotaEncomenda { get; set; } = null!;

        public int MateriaPrimaId { get; set; }               // ← Chave FK
        public virtual MateriaPrima MateriaPrima { get; set; } = null!;

        public int Quantidade { get; set; }
        public double PrecoUnit { get; set; }
    }
}