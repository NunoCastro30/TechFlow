using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogisControlAPI.Models;
using LogisControlAPI.Data;
using LogisControlAPI.DTO;


namespace LogisControlAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão dos pedidos de compra.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoCompraController : ControllerBase
    {
        private readonly LogisControlContext _context;

        /// <summary>
        /// Construtor que injeta o contexto da base de dados.
        /// </summary>
        public PedidoCompraController(LogisControlContext context)
        {
            _context = context;
        }

        #region CriarPedidoCompra
        /// <summary>
        /// Cria um novo pedido de compra.
        /// </summary>
        /// <param name="dto">Dados para criação do pedido.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="201">Pedido de compra criado com sucesso.</response>
        /// <response code="400">Utilizador não encontrado ou dados inválidos.</response>
        /// <response code="500">Erro interno ao criar o pedido de compra.</response>
        [HttpPost("CriarPedidoCompra")]
        public async Task<IActionResult> CriarPedidoCompra([FromBody] CriarPedidoCompraDTO dto)
        {
            try
            {
                // Verifica se o utilizador existe
                var utilizador = await _context.Utilizadores.FindAsync(dto.UtilizadorId);
                if (utilizador == null)
                    return BadRequest("Utilizador não encontrado.");

                var novoPedido = new PedidoCompra
                {
                    Descricao = dto.Descricao,
                    Estado = "Aberto",
                    DataAbertura = DateTime.UtcNow,
                    UtilizadorUtilizadorId = dto.UtilizadorId
                };

                _context.PedidosCompra.Add(novoPedido);
                await _context.SaveChangesAsync();

                return StatusCode(201, "Pedido de compra criado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar pedido de compra: {ex.Message}");
            }
        }
        #endregion

        #region ListarPedidosCompra
        /// <summary>
        /// Lista todos os pedidos de compra com o nome do utilizador associado.
        /// </summary>
        /// <returns>Lista de pedidos de compra.</returns>
        /// <response code="200">Lista obtida com sucesso.</response>
        /// <response code="500">Erro ao obter os pedidos.</response>
        [HttpGet("ListarPedidoCompra")]
        public async Task<ActionResult<IEnumerable<PedidoCompraDTO>>> ListarPedidosCompra()
        {
            try
            {
                var pedidos = await _context.PedidosCompra
                    .Include(p => p.UtilizadorUtilizador)
                    .Select(p => new PedidoCompraDTO
                    {
                        PedidoCompraId = p.PedidoCompraId,
                        Descricao = p.Descricao,
                        Estado = p.Estado,
                        DataAbertura = p.DataAbertura,
                        DataConclusao = p.DataConclusao,
                        NomeUtilizador = $"{p.UtilizadorUtilizador.PrimeiroNome} {p.UtilizadorUtilizador.Sobrenome}"
                    })
                    .ToListAsync();

                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter pedidos de compra: {ex.Message}");
            }
        }
        #endregion

        #region AtualizarEstadoPedido
        /// <summary>
        /// Atualiza o estado de um pedido de compra. Se o estado for "Concluído", define também a data de conclusão.
        /// </summary>
        /// <param name="pedidoId">ID do pedido a atualizar.</param>
        /// <param name="dto">Novo estado.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Estado atualizado com sucesso.</response>
        /// <response code="404">Pedido de compra não encontrado.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="500">Erro interno ao atualizar o estado.</response>
        [HttpPut("AtualizarEstado/{pedidoId}")]
        public async Task<IActionResult> AtualizarEstadoPedido(int pedidoId, [FromBody] AtualizarEstadoPedidoDTO dto)
        {
            try
            {
                var pedido = await _context.PedidosCompra.FindAsync(pedidoId);
                if (pedido == null)
                    return NotFound("Pedido de compra não encontrado.");

                pedido.Estado = dto.Estado;

                if (dto.Estado.ToLower() == "fechado")
                {
                    pedido.DataConclusao = DateTime.UtcNow;
                }
                else
                {
                    pedido.DataConclusao = null; // limpa se voltar a estado anterior
                }

                await _context.SaveChangesAsync();
                return Ok("Estado atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar estado do pedido: {ex.Message}");
            }
        }
        #endregion

        #region AtualizarDescricao
        /// <summary>
        /// Atualiza a descrição de um pedido de compra.
        /// </summary>
        /// <param name="pedidoId">ID do pedido.</param>
        /// <param name="dto">Nova descrição.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Descrição atualizada com sucesso.</response>
        /// <response code="404">Pedido não encontrado.</response>
        /// <response code="500">Erro interno.</response>
        [HttpPut("AtualizarDescricao/{pedidoId}")]
        public async Task<IActionResult> AtualizarDescricao(int pedidoId, [FromBody] AtualizarDescricaoPedidoDTO dto)
        {
            try
            {
                var pedido = await _context.PedidosCompra.FindAsync(pedidoId);
                if (pedido == null)
                    return NotFound("Pedido de compra não encontrado.");

                pedido.Descricao = dto.NovaDescricao;
                await _context.SaveChangesAsync();

                return Ok("Descrição atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar a descrição: {ex.Message}");
            }
        }
        #endregion

    }
}