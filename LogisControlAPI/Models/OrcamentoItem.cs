using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models;

public partial class OrcamentoItem
{
    public int OrcamentoItemId { get; set; }

    public int Quantidade { get; set; }

    public double PrecoUnit { get; set; }

    public int? PrazoEntrega { get; set; }

    public int OrcamentoOrcamentoId { get; set; }

    public int MateriaPrimaMateriaPrimalD { get; set; }

    public virtual MateriaPrima MateriaPrimaMateriaPrimalDNavigation { get; set; } = null!;

    public virtual Orcamento OrcamentoOrcamento { get; set; } = null!;
}
