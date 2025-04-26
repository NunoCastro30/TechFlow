using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão dos pedidos de cotação.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoCotacaoController : ControllerBase
    {
        private readonly LogisControlContext _context;

        /// <summary>
        /// Propriedade para acessar a tabela de pedidos de cotação no banco de dados.
        /// </summary>
        public DbSet<PedidoCotacao> PedidosDeCotacao { get; set; }

        /// <summary>
        /// Construtor do controlador que injeta o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da base de dados.</param>
        public PedidoCotacaoController(LogisControlContext context)
        {
            _context = context;
        }

        #region ObterPedidosDeCotacao
        /// <summary>
        /// Obtém a lista de todos os pedidos de cotação registados.
        /// </summary>
        /// <returns>Lista de pedidos de cotação existentes.</returns>
        /// <response code="200">Retorna a lista de pedidos de cotação com sucesso.</response>
        /// <response code="500">Erro interno ao tentar obter pedidos de cotação.</response>
        [HttpGet("GetPedidoCotacao")]
        public async Task<ActionResult<IEnumerable<PedidoCotacaoDTO>>> GetPedidosDeCotacao()
        {
            try
            {
                var pedidosDeCotacao = await _context.PedidosCotacao
                    .Select(p => new PedidoCotacaoDTO
                    {
                        PedidoCotacaoId = p.PedidoCotacaoId,
                        Descricao = p.Descricao,
                        Data = p.Data,
                        Estado = p.Estado,
                        FornecedorFornecedorId = p.FornecedorFornecedorId
                    })
                    .ToListAsync();

                return Ok(pedidosDeCotacao);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter pedidos de cotação: {ex.Message}");
            }
        }
        #endregion

        #region ObterPedidoDeCotacaoPorId
        /// <summary>
        /// Obtém um pedido de cotação pelo seu ID.
        /// </summary>
        /// <param name="id">Identificador do pedido de cotação.</param>
        /// <returns>Dados do pedido de cotação correspondente.</returns>
        /// <response code="200">Pedido de cotação encontrado com sucesso.</response>
        /// <response code="404">Pedido de cotação não encontrado.</response>
        [HttpGet("GetPedidoDeCotacao/{id}")]
        public async Task<ActionResult<PedidoCotacaoDTO>> GetPedidoDeCotacao(int id)
        {
            var pedidoDeCotacao = await _context.PedidosCotacao.FindAsync(id);
            if (pedidoDeCotacao == null)
            {
                return NotFound();
            }
            return Ok(new PedidoCotacaoDTO
            {
                //PedidoCotacaoId = pedidoDeCotacao.PedidoCotacaoId,
                Descricao = pedidoDeCotacao.Descricao,
                Data = pedidoDeCotacao.Data,
                Estado = pedidoDeCotacao.Estado,
                FornecedorFornecedorId = pedidoDeCotacao.FornecedorFornecedorId
            });
        }
        #endregion

        #region CriarPedidoDeCotacao
        /// <summary>
        /// Cria um novo pedido de cotação.
        /// </summary>
        /// <param name="pedidoDeCotacaoDTO">Dados para a criação do pedido de cotação.</param>
        /// <returns>Pedido de cotação criado com sucesso.</returns>
        /// <response code="201">Pedido de cotação criado com sucesso.</response>
        /// <response code="400">Dados inválidos para o pedido de cotação.</response>
        /// <response code="500">Erro interno ao criar o pedido de cotação.</response>
        [HttpPost]
        public async Task<ActionResult<PedidoCotacaoDTO>> CreatePedidoDeCotacao([FromBody] PedidoCotacaoDTO pedidoDeCotacaoDTO)
        {
            try
            {
                var pedidoDeCotacao = new PedidoCotacao
                {
                    Descricao = pedidoDeCotacaoDTO.Descricao,
                    Data = pedidoDeCotacaoDTO.Data,
                    Estado = pedidoDeCotacaoDTO.Estado,
                    FornecedorFornecedorId = pedidoDeCotacaoDTO.FornecedorFornecedorId
                };

                _context.PedidosCotacao.Add(pedidoDeCotacao);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPedidoDeCotacao), new { id = pedidoDeCotacao.PedidoCotacaoId }, pedidoDeCotacao);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao criar pedido de cotação: {ex.Message}");
            }
        }
        #endregion

        #region AtualizarPedidoDeCotacao
        /// <summary>
        /// Atualiza os dados de um pedido de cotação existente.
        /// </summary>
        /// <param name="id">ID do pedido de cotação a atualizar.</param>
        /// <param name="pedidoDeCotacaoDTO">Dados atualizados do pedido de cotação.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="204">Pedido de cotação atualizado com sucesso.</response>
        /// <response code="404">Pedido de cotação não encontrado.</response>
        /// <response code="500">Erro interno ao tentar atualizar o pedido de cotação.</response>
        [HttpPut("UpdatePedidoDeCotacao/{id}")]
        public async Task<IActionResult> UpdatePedidoDeCotacao(int id, [FromBody] PedidoCotacaoDTO pedidoDeCotacaoDTO)
        {
            try
            {
                var pedidoDeCotacao = await _context.PedidosCotacao.FindAsync(id);
                if (pedidoDeCotacao == null)
                {
                    return NotFound();
                }

                pedidoDeCotacao.Descricao = pedidoDeCotacaoDTO.Descricao;
                pedidoDeCotacao.Data = pedidoDeCotacaoDTO.Data;
                pedidoDeCotacao.Estado = pedidoDeCotacaoDTO.Estado;
                pedidoDeCotacao.FornecedorFornecedorId = pedidoDeCotacaoDTO.FornecedorFornecedorId;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar pedido de cotação: {ex.Message}");
            }
        }
        #endregion

        #region DeletarPedidoDeCotacao
        /// <summary>
        /// Remove um pedido de cotação existente.
        /// </summary>
        /// <param name="id">ID do pedido de cotação a remover.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="204">Pedido de cotação removido com sucesso.</response>
        /// <response code="404">Pedido de cotação não encontrado.</response>
        /// <response code="500">Erro interno ao tentar remover o pedido de cotação.</response>
        [HttpDelete("DeletePedidoDeCotacao/{id}")]
        public async Task<IActionResult> DeletePedidoDeCotacao(int id)
        {
            try
            {
                var pedidoDeCotacao = await _context.PedidosCotacao.FindAsync(id);
                if (pedidoDeCotacao == null)
                {
                    return NotFound();
                }

                _context.PedidosCotacao.Remove(pedidoDeCotacao);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao remover pedido de cotação: {ex.Message}");
            }
        }
        #endregion
    }
}
