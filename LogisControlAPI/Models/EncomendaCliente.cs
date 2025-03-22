using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models
{
    /// <summary>
    /// Representa uma encomenda realizada por um cliente, com data, estado e ligação a itens e ordens de produção.
    /// </summary>
    public partial class EncomendaCliente
    {
        /// <summary>
        /// Identificador único da encomenda do cliente.
        /// </summary>
        public int EncomendaClienteId { get; set; }

        /// <summary>
        /// Data em que a encomenda foi registada.
        /// </summary>
        public DateTime DataEncomenda { get; set; }

        /// <summary>
        /// Estado atual da encomenda (ex: Pendente, Em Produção, Concluída).
        /// </summary>
        public string Estado { get; set; } = null!;

        /// <summary>
        /// Chave estrangeira que referencia o cliente que fez a encomenda.
        /// </summary>
        public int ClienteClienteId { get; set; }

        /// <summary>
        /// Referência ao cliente associado a esta encomenda.
        /// </summary>
        public virtual Cliente ClienteCliente { get; set; } = null!;

        /// <summary>
        /// Lista de itens incluídos nesta encomenda.
        /// </summary>
        public virtual ICollection<EncomendaIten> EncomendaItens { get; set; } = new List<EncomendaIten>();

        /// <summary>
        /// Lista de ordens de produção geradas a partir desta encomenda.
        /// </summary>
        public virtual ICollection<OrdemProducao> OrdemProducaos { get; set; } = new List<OrdemProducao>();
    }
}