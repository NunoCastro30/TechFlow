using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models;

public partial class MateriaPrima
{
    public int MateriaPrimalD { get; set; }

    public string Nome { get; set; } = null!;

    public int Quantidade { get; set; }

    public string Descricao { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public string CodInterno { get; set; } = null!;

    public double Preco { get; set; }

    public virtual ICollection<MateriaPrimaProduto> MateriaPrimaProdutos { get; set; } = new List<MateriaPrimaProduto>();

    public virtual ICollection<NotaEncomendaIten> NotaEncomendaItens { get; set; } = new List<NotaEncomendaIten>();

    public virtual ICollection<OrcamentoItem> OrcamentoItems { get; set; } = new List<OrcamentoItem>();

    public virtual ICollection<ProdMateriai> ProdMateriais { get; set; } = new List<ProdMateriai>();
}
