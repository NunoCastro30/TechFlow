using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models;

public partial class Orcamento
{
    public int OrcamentoId { get; set; }

    public DateTime Data { get; set; }

    public string Estado { get; set; } = null!;

    public int PedidoCotacaoPedidoCotacaoId { get; set; }

    public virtual ICollection<NotaEncomenda> NotaEncomenda { get; set; } = new List<NotaEncomenda>();

    public virtual ICollection<OrcamentoItem> OrcamentosItem { get; set; } = new List<OrcamentoItem>();

    public virtual PedidoCotacao PedidoCotacaoPedidoCotacao { get; set; } = null!;
}
