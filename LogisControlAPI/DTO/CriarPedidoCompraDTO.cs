using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LogisControlAPI.DTO
{
    /// <summary>
    /// DTO utilizado para criar um novo pedido de compra, incluindo as linhas de itens.
    /// </summary>
    public class CriarPedidoCompraDTO
    {
        /// <summary>Descrição do pedido de compra.</summary>
        [Required]
        public string Descricao { get; set; } = null!;

        /// <summary>ID do utilizador que está a criar o pedido.</summary>
        [Required]
        public int UtilizadorId { get; set; }

        /// <summary>
        /// Lista de itens (matéria-prima + quantidade) que compõem este pedido.
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Deve indicar pelo menos um item no pedido.")]
        public List<ItemPedidoDTO> Itens { get; set; } = new();
    }

    /// <summary>
    /// DTO que representa uma linha (item) de um pedido de compra.
    /// </summary>
    public class ItemPedidoDTO
    {
        /// <summary>ID da matéria-prima (FK para MateriaPrima).</summary>
        [Required]
        public int MateriaPrimaId { get; set; }

        /// <summary>Quantidade da matéria-prima pretendida.</summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser pelo menos 1.")]
        public int Quantidade { get; set; }
    }
}