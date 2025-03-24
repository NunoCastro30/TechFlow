using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models
{
    /// <summary>
    /// Representa um cliente da empresa, com os respetivos dados de identificação e morada.
    /// </summary>
    public partial class Cliente
    {
        /// <summary>
        /// Identificador único do cliente.
        /// </summary>
        public int ClienteId { get; set; }

        /// <summary>
        /// Nome completo ou designação do cliente.
        /// </summary>
        public string Nome { get; set; } = null!;

        /// <summary>
        /// Número de Identificação Fiscal (NIF) do cliente.
        /// </summary>
        public int Nif { get; set; }

        /// <summary>
        /// Morada completa do cliente.
        /// </summary>
        public string Morada { get; set; } = null!;

        /// <summary>
        /// Lista de encomendas realizadas por este cliente.
        /// </summary>
        public virtual ICollection<EncomendaCliente> EncomendaClientes { get; set; } = new List<EncomendaCliente>();
    }
}
   public virtual ICollection<EncomendaCliente> EncomendasCliente { get; set; } = new List<EncomendaCliente>();
}
