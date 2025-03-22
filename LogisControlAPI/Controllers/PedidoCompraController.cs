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
                var utilizador = await _context.Utilizadors.FindAsync(dto.UtilizadorId);
                if (utilizador == null)
                    return BadRequest("Utilizador não encontrado.");

                var novoPedido = new PedidoCompra
                {
                    Descricao = dto.Descricao,
                    Estado = "Aberto",
                    DataAbertura = DateTime.UtcNow,
                    UtilizadorUtilizadorId = dto.UtilizadorId
                };

                _context.PedidoCompras.Add(novoPedido);
                await _context.SaveChangesAsync();

                return StatusCode(201, "Pedido de compra criado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar pedido de compra: {ex.Message}");
            }
        }
        #endregion

        #region AtulizarPedidoDeCompra

        #endregion 

    }
}