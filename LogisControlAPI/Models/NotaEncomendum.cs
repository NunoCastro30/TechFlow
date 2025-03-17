using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models;

public partial class NotaEncomendum
{
    public int NotaEncomendaId { get; set; }

    public DateTime DataEmissao { get; set; }

    public string Estado { get; set; } = null!;

    public double ValorTotal { get; set; }

    public int OrcamentoOrcamentoId { get; set; }

    public virtual ICollection<NotaEncomendaIten> NotaEncomendaItens { get; set; } = new List<NotaEncomendaIten>();

    public virtual Orcamento OrcamentoOrcamento { get; set; } = null!;
}
