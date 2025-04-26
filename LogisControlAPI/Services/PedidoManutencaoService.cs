using LogisControlAPI.Data;
using LogisControlAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Services
{
    public class PedidoManutencaoService
    {
        private readonly LogisControlContext _context;

        public PedidoManutencaoService(LogisControlContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Devolve os pedidos de manutenção com mais de 7 dias e ainda não concluídos.
        /// </summary>
        public async Task<List<PedidoManutencao>> ObterPedidosAtrasadosAsync()
        {
            var hoje = DateTime.Now;
            return await _context.PedidosManutencao
                .Where(p => p.Estado != "Resolvido" && EF.Functions.DateDiffDay(p.DataAbertura, hoje) > 7)
                .ToListAsync();
        }
    }
}