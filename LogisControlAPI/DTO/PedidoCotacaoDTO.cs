using LogisControlAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.DTO
{
    public class CriarPedidoCotacaoDTO
    {

        public string Descricao { get; set; } = null!;

        public DateTime Data { get; set; }

        public string? Estado { get; set; }
        public int FornecedorFornecedorId { get; set; }

    }
}
