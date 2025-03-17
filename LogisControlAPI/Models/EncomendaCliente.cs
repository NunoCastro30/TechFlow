using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models;

public partial class EncomendaCliente
{
    public int EncomendaClienteId { get; set; }

    public DateTime DataEncomenda { get; set; }

    public string Estado { get; set; } = null!;

    public int ClienteClienteId { get; set; }

    public virtual Cliente ClienteCliente { get; set; } = null!;

    public virtual ICollection<EncomendaIten> EncomendaItens { get; set; } = new List<EncomendaIten>();

    public virtual ICollection<OrdemProducao> OrdemProducaos { get; set; } = new List<OrdemProducao>();
}
