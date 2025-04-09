using System.ComponentModel.DataAnnotations.Schema;

namespace LogisControlAPI.DTO
{
    public partial class MateriaPrimaDTO
    {
        public int MateriaPrimaId { get; set; }

        public string Nome { get; set; } = null!;

        public int Quantidade { get; set; }

        public string Descricao { get; set; } = null!;

        public string Categoria { get; set; } = null!;

        public string CodInterno { get; set; } = null!;

        public double Preco { get; set; }
    }
}
