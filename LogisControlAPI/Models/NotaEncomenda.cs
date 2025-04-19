using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models;

public partial class NotaEncomenda
{
    public int NotaEncomendaId { get; set; }

    public DateTime DataEmissao { get; set; }

    public string Estado { get; set; } = null!;

    public double ValorTotal { get; set; }

    public int OrcamentoOrcamentoId { get; set; }

    public virtual ICollection<NotaEncomendaItens> NotasEncomendaItem { get; set; } = new List<NotaEncomendaItens>();

    public virtual Orcamento OrcamentoOrcamento { get; set; } = null!;
}
