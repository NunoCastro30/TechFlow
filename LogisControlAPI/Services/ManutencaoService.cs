using LogisControlAPI.Data;
using LogisControlAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Services
{
    public class ManutencaoService
    {
        private readonly LogisControlContext _context;

        public ManutencaoService(LogisControlContext context)
        {
            _context = context;
        }

        #region Operações de Pedidos de Manutenção

        /// <summary>
        /// Devolve os pedidos de manutenção com mais de 7 dias e ainda não concluídos.
        /// </summary>
        /// <returns>Lista de pedidos de manutenção em atraso</returns>
        /// <remarks>
        /// Considera como "em atraso" qualquer pedido que não esteja no estado "Resolvido"
        /// e cuja data de abertura seja anterior a 7 dias da data atual.
        /// </remarks>
        public async Task<List<PedidoManutencao>> ObterPedidosAtrasadosAsync()
        {
            var hoje = DateTime.Now;
            return await _context.PedidosManutencao
                .Where(p => p.Estado != "Resolvido" && EF.Functions.DateDiffDay(p.DataAbertura, hoje) > 7)
                .ToListAsync();
        }

        /// <summary>
        /// Atualiza o estado de um pedido de manutenção e define automaticamente a data de conclusão
        /// quando o estado for "Recusado".
        /// </summary>
        /// <param name="pedidoId">ID do pedido de manutenção a ser atualizado</param>
        /// <param name="novoEstado">Novo estado do pedido (deve ser um valor válido)</param>
        /// <exception cref="Exception">Lançada quando o pedido não é encontrado</exception>
        /// <remarks>
        /// Esta função garante que quando um pedido é recusado, a data de conclusão é automaticamente
        /// preenchida com a data/hora atual do servidor.
        /// </remarks>
        public async Task AtualizarEstadoPedido(int pedidoId, string novoEstado)
        {
            var pedido = await _context.PedidosManutencao.FindAsync(pedidoId);
            if (pedido == null) throw new Exception("Pedido não encontrado");

            pedido.Estado = novoEstado;

            // Atualiza dataConclusao se for "Recusado"
            if (novoEstado is "Recusado")
            {
                pedido.DataConclusao = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        #endregion

        #region Operações de Registos de Manutenção

        /// <summary>
        /// Verifica se um registo de manutenção foi marcado como "Resolvido" e, em caso afirmativo,
        /// atualiza automaticamente o estado do pedido associado para "Concluido" com a data atual.
        /// </summary>
        /// <param name="registoId">ID do registo de manutenção a verificar</param>
        /// <exception cref="Exception">
        /// Lançada quando o registo ou o pedido associado não são encontrados
        /// </exception>
        /// <remarks>
        /// Esta função é tipicamente chamada após a atualização de um registo de manutenção
        /// para garantir a sincronização entre o estado do registo e do pedido associado.
        /// </remarks>
        public async Task AtualizarEstadoPedidoSeRegistoResolvido(int registoId)
        {
            // Buscar o registo
            var registo = await _context.RegistosManutencao.FindAsync(registoId);
            if (registo == null) throw new Exception("Registo não encontrado.");

            // Se o estado for "Resolvido", atualiza o pedido associado
            if (registo.Estado == "Resolvido")
            {
                var pedido = await _context.PedidosManutencao.FindAsync(registo.PedidoManutencaoPedidoManutId);
                if (pedido == null) throw new Exception("Pedido de manutenção associado não encontrado.");

                pedido.Estado = "Concluido";
                pedido.DataConclusao = DateTime.Now;

                await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}