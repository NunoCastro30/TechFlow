namespace LogisControlAPI.DTO
{
    /// <summary>
    /// Dados para criar um pedido de cotação:
    /// qual pedido de compra e a que fornecedor.
    /// </summary>
    public class CriarPedidoCotacaoDTO
    {
        /// <summary>ID do pedido de compra a cotar.</summary>
        public int PedidoCompraId { get; set; }

        /// <summary>ID do fornecedor a quem se dirige a cotação.</summary>
        public int FornecedorId { get; set; }
    }
}