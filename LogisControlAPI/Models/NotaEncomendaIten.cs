using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models;

public partial class NotaEncomendaIten
{
    public int NotaEncomendaItensId { get; set; }

    public int Quantidade { get; set; }

    public double PrecoUnit { get; set; }

    public int NotaEncomendaNotaEncomendaId { get; set; }

    public int MateriaPrimaMateriaPrimalD { get; set; }

    public virtual MateriaPrima MateriaPrimaMateriaPrimalDNavigation { get; set; } = null!;

    public virtual NotaEncomendum NotaEncomendaNotaEncomenda { get; set; } = null!;
}
