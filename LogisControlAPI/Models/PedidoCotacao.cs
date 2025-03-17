using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models;

public partial class PedidoCotacao
{
    public int PedidoCotacaoId { get; set; }

    public string Descricao { get; set; } = null!;

    public DateTime Data { get; set; }

    public string? Estado { get; set; }

    public int FornecedorFornecedorId { get; set; }

    public virtual Fornecedor FornecedorFornecedor { get; set; } = null!;

    public virtual ICollection<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();
}
