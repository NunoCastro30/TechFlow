﻿using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models;

public partial class EncomendaIten
{
    public int EncomendaItens { get; set; }

    public int? Quantidade { get; set; }

    public int EncomendaClienteEncomendaClienteId { get; set; }

    public virtual EncomendaCliente EncomendaClienteEncomendaCliente { get; set; } = null!;

    public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}
