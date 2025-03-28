﻿using Microsoft.AspNetCore.Mvc;
using LogisControlAPI.Models;
using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão dos pedidos de manutenção.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoManutencaoController : ControllerBase
    {
        private readonly LogisControlContext _context;

        /// <summary>
        /// Construtor do controlador que injeta o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da base de dados.</param>
        public PedidoManutencaoController(LogisControlContext context)
        {
            _context = context;
        }

        #region ObterPedidos
        /// <summary>
        /// Obtém a lista de todos os pedidos de manutenção registados.
        /// </summary>
        /// <returns>Lista de pedidos de manutenção.</returns>
        /// <response code="200">Retorna a lista de pedidos com sucesso.</response>
        /// <response code="500">Erro interno ao tentar obter os pedidos.</response>
        [HttpGet("ObterPedidos")]
        public async Task<ActionResult<IEnumerable<PedidoManutençãoDTO>>> GetPedidos()
        {
            try
            {
                var pedidos = await _context.PedidosManutencao
                    .Select(p => new PedidoManutençãoDTO
                    {
                        PedidoManutId = p.PedidoManutId,
                        Descicao = p.Descicao,
                        Estado = p.Estado,
                        DataAbertura = p.DataAbertura,
                        DataConclusao = p.DataConclusao,
                        MaquinaMaquinaId = p.MaquinaMaquinaId,
                        UtilizadorUtilizadorId = p.UtilizadorUtilizadorId
                    })
                    .ToListAsync();

                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter pedidos de manutenção: {ex.Message}");
            }
        }
        #endregion

        #region ObterPedidoPorId
        /// <summary>
        /// Obtém um pedido de manutenção pelo seu ID.
        /// </summary>
        /// <param name="id">Identificador único do pedido.</param>
        /// <returns>Dados do pedido de manutenção correspondente.</returns>
        /// <response code="200">Pedido encontrado com sucesso.</response>
        /// <response code="404">Pedido não encontrado.</response>
        /// <response code="500">Erro interno ao procurar o pedido.</response>
        [HttpGet("ObterPedido/{id}")]
        public async Task<ActionResult<PedidoManutençãoDTO>> GetPedidoPorId(int id)
        {
            try
            {
                var pedido = await _context.PedidosManutencao
                    .Where(p => p.PedidoManutId == id)
                    .Select(p => new PedidoManutençãoDTO
                    {
                        PedidoManutId = p.PedidoManutId,
                        Descicao = p.Descicao,
                        Estado = p.Estado,
                        DataAbertura = p.DataAbertura,
                        DataConclusao = p.DataConclusao,
                        MaquinaMaquinaId = p.MaquinaMaquinaId,
                        UtilizadorUtilizadorId = p.UtilizadorUtilizadorId
                    })
                    .FirstOrDefaultAsync();

                if (pedido == null)
                    return NotFound("Pedido de manutenção não encontrado.");

                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter pedido: {ex.Message}");
            }
        }
        #endregion

        #region CriarPedido
        /// <summary>
        /// Cria um novo pedido de manutenção.
        /// </summary>
        /// <param name="novoPedidoDto">Dados para criação do pedido.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="201">Pedido de manutenção criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="500">Erro interno ao criar o pedido.</response>
        [HttpPost("CriarPedido")]
        public async Task<ActionResult> CriarPedido([FromBody] PedidoManutençãoDTO novoPedidoDto)
        {
            try
            {
                var novoPedido = new PedidoManutencao
                {
                    Descicao = novoPedidoDto.Descicao,
                    Estado = novoPedidoDto.Estado,
                    DataAbertura = novoPedidoDto.DataAbertura,
                    DataConclusao = novoPedidoDto.DataConclusao,
                    MaquinaMaquinaId = novoPedidoDto.MaquinaMaquinaId,
                    UtilizadorUtilizadorId = novoPedidoDto.UtilizadorUtilizadorId
                };

                _context.PedidosManutencao.Add(novoPedido);
                await _context.SaveChangesAsync();

                return StatusCode(201, "Pedido de manutenção criado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao criar pedido: {ex.Message}");
            }
        }
        #endregion

        #region AtualizarPedido
        /// <summary>
        /// Atualiza os dados de um pedido de manutenção existente.
        /// </summary>
        /// <param name="pedidoId">ID do pedido a atualizar.</param>
        /// <param name="pedidoAtualizado">Dados atualizados do pedido.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Pedido atualizado com sucesso.</response>
        /// <response code="404">Pedido não encontrado.</response>
        /// <response code="500">Erro interno ao tentar atualizar o pedido.</response>
        [HttpPut("AtualizarPedido/{pedidoId}")]
        public async Task<IActionResult> AtualizarPedido(int pedidoId, [FromBody] PedidoManutençãoDTO pedidoAtualizado)
        {
            try
            {
                var pedido = await _context.PedidosManutencao.FindAsync(pedidoId);

                if (pedido == null)
                    return NotFound("Pedido de manutenção não encontrado.");

                // Atualizar os campos
                pedido.Descicao = pedidoAtualizado.Descicao;
                pedido.Estado = pedidoAtualizado.Estado;
                pedido.DataAbertura = pedidoAtualizado.DataAbertura;
                pedido.DataConclusao = pedidoAtualizado.DataConclusao;
                pedido.MaquinaMaquinaId = pedidoAtualizado.MaquinaMaquinaId;
                pedido.UtilizadorUtilizadorId = pedidoAtualizado.UtilizadorUtilizadorId;

                await _context.SaveChangesAsync();

                return Ok("Pedido de manutenção atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar pedido: {ex.Message}");
            }
        }
        #endregion
    }
}

