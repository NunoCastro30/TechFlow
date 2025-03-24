namespace LogisControlAPI.DTO
{
    public partial class RegistoProducaoDTO
    {
        public int RegistoProducaoId { get; set; }

        public string Estado { get; set; }

        public DateTime DataProducao { get; set; }

        public string? Observacoes { get; set; } = null!;

        public int UtilizadorUtilizadorId { get; set; }

        public int ProdutoProdutoId { get; set; }

        public int OrdemProducaoOrdemProdId { get; set; }
    }
}
