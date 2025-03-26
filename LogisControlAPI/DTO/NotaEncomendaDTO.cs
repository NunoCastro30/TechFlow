namespace LogisControlAPI.DTO
{
    public class NotaEncomendaDTO
    {


        public DateTime DataEmissao { get; set; }

        public string Estado { get; set; } = null!;

        public double ValorTotal { get; set; }

        public int OrcamentoOrcamentoId { get; set; }

    }
}
