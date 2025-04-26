using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models;

public partial class PedidoCotacao
{
    public int PedidoCotacaoId { get; set; }
    public string Descricao { get; set; } = null!;
    public DateTime Data { get; set; }
    public string Estado { get; set; } = null!;

    // FK pro fornecedor que vai responder à cotação
    public int FornecedorId { get; set; }

    // NOVO: token para acesso único
    public string TokenAcesso { get; set; } = null!;

    public virtual Fornecedor Fornecedor { get; set; } = null!;

    // navegação para orçamentos, se quiser depois incluir orçamentos filhos
    public virtual ICollection<Orcamento> Orcamentos { get; set; } = new List<Orcamento>();
}