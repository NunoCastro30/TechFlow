using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models;

public partial class ProdMateriai
{
    public int ProdMateriaisId { get; set; }

    public int QuantidadeUtilizada { get; set; }

    public int OrdemProducaoOrdemProdId { get; set; }

    public int MateriaPrimaMateriaPrimalD { get; set; }

    public virtual MateriaPrima MateriaPrimaMateriaPrimalDNavigation { get; set; } = null!;

    public virtual OrdemProducao OrdemProducaoOrdemProd { get; set; } = null!;
}
