using System;
using System.Collections.Generic;

namespace LogisControlAPI.DTO
{
    /// <summary>
    /// DTO que traz o detalhe completo de um pedido de compra, incluindo itens.
    /// </summary>
    public class PedidoCompraDetalheDTO
    {
        /// <summary>ID do pedido de compra.</summary>
        public int PedidoCompraId { get; set; }

        /// <summary>Descrição do pedido.</summary>
        public string Descricao { get; set; } = null!;

        /// <summary>Estado atual do pedido.</summary>
        public string Estado { get; set; } = null!;

        /// <summary>Data em que o pedido foi aberto.</summary>
        public DateTime DataAbertura { get; set; }

        /// <summary>Data em que o pedido foi concluído (pode ser nulo se não estiver fechado).</summary>
        public DateTime? DataConclusao { get; set; }

        /// <summary>Nome completo do utilizador que criou o pedido.</summary>
        public string NomeUtilizador { get; set; } = null!;

        /// <summary>Itens associados ao pedido, com nome e quantidade.</summary>
        public List<ItemPedidoDetalheDTO> Itens { get; set; } = new();
    }

    /// <summary>
    /// DTO que representa o detalhe de cada item na visão detalhada do pedido.
    /// </summary>
    public class ItemPedidoDetalheDTO
    {
        /// <summary>ID da matéria-prima.</summary>
        public int MateriaPrimaId { get; set; }

        /// <summary>Nome da matéria-prima.</summary>
        public string MateriaPrimaNome { get; set; } = null!;

        /// <summary>Quantidade encomendada.</summary>
        public int Quantidade { get; set; }
    }
}
