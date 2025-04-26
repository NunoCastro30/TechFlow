using LogisControlAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.DTO
{
    public class PedidoCotacaoDTO
    {
        public int PedidoCotacaoId { get; set; }

        public string Descricao { get; set; } = null!;

        public DateTime Data { get; set; }

        public string? Estado { get; set; }

        public int FornecedorFornecedorId { get; set; }

    }
}
