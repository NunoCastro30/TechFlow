namespace LogisControlAPI.DTO
{
    public class OrcamentoDTO
    {

        public int OrcamentoId { get; set; }

        public DateTime Data { get; set; }

        public string Estado { get; set; } = null!;

        public int PedidoCotacaoPedidoCotacaoId { get; set; }
    }
}
