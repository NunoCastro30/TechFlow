using System;

namespace LogisControlAPI.DTO
{
    /// <summary>
    /// DTO para listagem resumida de pedidos de compra.
    /// </summary>
    public class PedidoCompraDTO
    {
        /// <summary>ID do pedido de compra.</summary>
        public int PedidoCompraId { get; set; }

        /// <summary>Descrição do pedido.</summary>
        public string Descricao { get; set; } = null!;

        /// <summary>Estado atual do pedido.</summary>
        public string Estado { get; set; } = null!;

        /// <summary>Data de abertura do pedido.</summary>
        public DateTime DataAbertura { get; set; }

        /// <summary>Data de conclusão (quando aplicável).</summary>
        public DateTime? DataConclusao { get; set; }

        /// <summary>Nome completo do utilizador que criou o pedido.</summary>
        public string NomeUtilizador { get; set; } = null!;
    }
}