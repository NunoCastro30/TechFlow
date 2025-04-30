namespace LogisControlAPI.DTO
{
    public class ProdutoListagemDTO
    {
        public string CodInterno { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public int Quantidade { get; set; }
        public string Descricao { get; set; } = null!;
        public double Preco { get; set; }
    }
}
