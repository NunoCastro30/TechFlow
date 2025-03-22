﻿using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models;

public partial class Produto
{
    public int ProdutoId { get; set; }

    public string Nome { get; set; } = null!;

    public string Quantidade { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public string CodInterno { get; set; } = null!;

    public double Preco { get; set; }

    public int OrdemProducaoOrdemProdId { get; set; }

    public int EncomendaItensEncomendaItensId { get; set; }

    public virtual EncomendaItens EncomendaItensEncomendaItensNavigation { get; set; } = null!;

    public virtual ICollection<MateriaPrimaProduto> MateriaPrimaProdutos { get; set; } = new List<MateriaPrimaProduto>();

    public virtual OrdemProducao OrdemProducaoOrdemProd { get; set; } = null!;

    public virtual ICollection<RegistoProducao> RegistosProducao { get; set; } = new List<RegistoProducao>();
}
