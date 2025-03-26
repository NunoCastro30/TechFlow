namespace LogisControlAPI.DTO
{
    public class OrcamentoItemDTO
    {
        public int OrcamentoItemId { get; set; }

        public int Quantidade { get; set; }

        public double PrecoUnit { get; set; }

        public int? PrazoEntrega { get; set; }

        public int OrcamentoOrcamentoId { get; set; }

        public int MateriaPrimaMateriaPrimaId { get; set; }
    }
}
