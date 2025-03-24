using System.ComponentModel.DataAnnotations;

namespace LogisControlAPI.DTO
{
    /// <summary>
    /// DTO utilizado para criar um novo pedido de compra.
    /// </summary>
    public class CriarPedidoCompraDTO
    {
        /// <summary>
        /// Descrição do pedido de compra.
        /// </summary>
        [Required]
        public string Descricao { get; set; }

        /// <summary>
        /// ID do utilizador que está a criar o pedido.
        /// </summary>
        [Required]
        public int UtilizadorId { get; set; }
    }
}
