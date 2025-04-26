using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisControlAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão dos orçamentos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrcamentoController : ControllerBase
    {
        private readonly LogisControlContext _context;

        /// <summary>
        /// Construtor do controlador que injeta o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da base de dados.</param>
        public OrcamentoController(LogisControlContext context)
        {
            _context = context;
        }

        #region ObterOrcamentos
        /// <summary>
        /// Obtém a lista de todos os orçamentos registados.
        /// </summary>
        /// <returns>Lista de orçamentos existentes.</returns>
        /// <response code="200">Retorna a lista de orçamentos com sucesso.</response>
        /// <response code="500">Erro interno ao tentar obter os orçamentos.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrcamentoDTO>>> GetOrcamentos()
        {
            try
            {
                var orcamentos = await _context.Orcamentos
                    .Select(o => new OrcamentoDTO
                    {
                        OrcamentoId=o.OrcamentoId,
                        Data = o.Data,
                        Estado = o.Estado,
                        PedidoCotacaoPedidoCotacaoId = o.PedidoCotacaoPedidoCotacaoId
                    })
                    .ToListAsync();

                return Ok(orcamentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter orçamentos: {ex.Message}");
            }
        }
        #endregion

        #region ObterOrcamentoPorId
        /// <summary>
        /// Obtém um orçamento pelo seu ID.
        /// </summary>
        /// <param name="id">Identificador do orçamento.</param>
        /// <returns>Dados do orçamento correspondente.</returns>
        /// <response code="200">Orçamento encontrado com sucesso.</response>
        /// <response code="404">Orçamento não encontrado.</response>
        [HttpGet("GetOrcamento{id}")]
        public async Task<ActionResult<OrcamentoDTO>> GetOrcamento(int id)
        {
            var orcamento = await _context.Orcamentos.FindAsync(id);
            if (orcamento == null)
            {
                return NotFound("Orçamento não encontrado.");
            }
            return Ok(new OrcamentoDTO
            {
               //OrcamentoId = orcamento.OrcamentoId,
                Data = orcamento.Data,
                Estado = orcamento.Estado,
                PedidoCotacaoPedidoCotacaoId = orcamento.PedidoCotacaoPedidoCotacaoId
            });
        }
        #endregion

        #region CriarOrcamento
        /// <summary>
        /// Cria um novo orçamento.
        /// </summary>
        /// <param name="orcamentoDTO">Dados para criação do orçamento.</param>
        /// <returns>Orçamento criado com sucesso.</returns>
        /// <response code="201">Orçamento criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="500">Erro interno ao criar o orçamento.</response>
        [HttpPost ("CriarOrcamento")]
        public async Task<ActionResult<OrcamentoDTO>> CreateOrcamento([FromBody] OrcamentoDTO orcamentoDTO)
        {
            try
            {
                var orcamento = new Orcamento
                {
                    Data = orcamentoDTO.Data,
                    Estado = orcamentoDTO.Estado,
                    PedidoCotacaoPedidoCotacaoId = orcamentoDTO.PedidoCotacaoPedidoCotacaoId
                };

                _context.Orcamentos.Add(orcamento);
                await _context.SaveChangesAsync();

                // Retorna o novo recurso criado com a rota para consulta
                return CreatedAtAction(nameof(GetOrcamento), new { id = orcamento.OrcamentoId }, orcamento);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao criar orçamento: {ex.Message}");
            }
        }
        #endregion

        #region AtualizarOrcamento
        /// <summary>
        /// Atualiza os dados de um orçamento existente.
        /// </summary>
        /// <param name="id">ID do orçamento a atualizar.</param>
        /// <param name="orcamentoDTO">Dados atualizados do orçamento.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="204">Orçamento atualizado com sucesso.</response>
        /// <response code="404">Orçamento não encontrado.</response>
        /// <response code="500">Erro interno ao tentar atualizar o orçamento.</response>
        [HttpPut("UpdateOrcamento/{id}")]
        public async Task<IActionResult> UpdateOrcamento(int id, [FromBody] OrcamentoDTO orcamentoDTO)
        {
            try
            {
                var orcamento = await _context.Orcamentos.FindAsync(id);
                if (orcamento == null)
                {
                    return NotFound("Orçamento não encontrado.");
                }

                // Atualiza os dados do orçamento
                orcamento.Data = orcamentoDTO.Data;
                orcamento.Estado = orcamentoDTO.Estado;
                orcamento.PedidoCotacaoPedidoCotacaoId = orcamentoDTO.PedidoCotacaoPedidoCotacaoId;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar orçamento: {ex.Message}");
            }
        }
        #endregion

        #region DeletarOrcamento
        /// <summary>
        /// Remove um orçamento existente.
        /// </summary>
        /// <param name="id">ID do orçamento a remover.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="204">Orçamento removido com sucesso.</response>
        /// <response code="404">Orçamento não encontrado.</response>
        /// <response code="500">Erro interno ao tentar remover o orçamento.</response>
        [HttpDelete("DeleteOrcamento/{id}")]
        public async Task<IActionResult> DeleteOrcamento(int id)
        {
            try
            {
                var orcamento = await _context.Orcamentos.FindAsync(id);
                if (orcamento == null)
                {
                    return NotFound("Orçamento não encontrado.");
                }

                _context.Orcamentos.Remove(orcamento);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao remover orçamento: {ex.Message}");
            }
        }
        #endregion
    }
}
