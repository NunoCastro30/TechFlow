using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models;

public partial class NotaEncomendaItens
{
    public int NotaEncomendaItensId { get; set; }

    public int Quantidade { get; set; }

    public double PrecoUnit { get; set; }

    public int NotaEncomendaNotaEncomendaId { get; set; }

    public int MateriaPrimaMateriaPrimaId { get; set; }

    public virtual MateriaPrima MateriaPrimaMateriaPrimaIDNavigation { get; set; } = null!;

    public virtual NotaEncomenda NotaEncomendaNotaEncomenda { get; set; } = null!;
}
