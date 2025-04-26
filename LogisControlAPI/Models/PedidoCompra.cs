using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models
{
    public partial class PedidoCompra
    {
        public int PedidoCompraId { get; set; }

        public string Descricao { get; set; } = null!;

        public string Estado { get; set; } = null!;

        public DateTime DataAbertura { get; set; }

        public DateTime? DataConclusao { get; set; }

        public int UtilizadorUtilizadorId { get; set; }

        /// <summary>
        /// Navegação para o utilizador que criou o pedido
        /// </summary>
        public virtual Utilizador UtilizadorUtilizador { get; set; } = null!;

        /// <summary>
        /// Linhas (itens) associadas a este pedido de compra.
        /// </summary>
        public virtual ICollection<PedidoCompraItem> PedidoCompraItems { get; set; }
            = new HashSet<PedidoCompraItem>();
    }
}
