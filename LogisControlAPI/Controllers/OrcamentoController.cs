using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using LogisControlAPI.Data;
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
    [Route("api/orcamentos")]
    public class OrcamentoController : ControllerBase
    {
        private readonly LogisControlContext _ctx;
        public OrcamentoController(LogisControlContext ctx) => _ctx = ctx;

        /// <summary>
        /// 1) Fornecedor cria um orçamento (cabeçalho) para um pedido de cotação.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CriarOrcamento([FromBody] OrcamentoDTO dto)
        {
            // 1.1) valida FK em PedidoCotacao
            var existePc = await _ctx.PedidosCotacao
                .AnyAsync(pc => pc.PedidoCotacaoId == dto.PedidoCotacaoID);
            if (!existePc)
                return BadRequest("Pedido de cotação inválido.");

            // 1.2) cria o cabeçalho
            var orc = new Orcamento
            {
                PedidoCotacaoPedidoCotacaoID = dto.PedidoCotacaoID,
                Data = DateTime.UtcNow,
                Estado = "Respondido"
            };
            _ctx.Orcamentos.Add(orc);
            await _ctx.SaveChangesAsync();

            // 1.3) devolve 201 Created com Location para GET /api/orcamentos/{orcId}
            return CreatedAtAction(
                nameof(ObterOrcamento),
                new { orcId = orc.OrcamentoID },
                new { OrcamentoId = orc.OrcamentoID }
            );
        }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter orçamentos: {ex.Message}");
            }
        }
        #endregion

        #region ObterOrcamentoPorId
        /// <summary>
        /// 2) Fornecedor adiciona linhas ao orçamento.
        /// </summary>
        [HttpPost("{orcId:int}/itens")]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AdicionarItem(
            [FromRoute] int orcId,
            [FromBody] OrcamentoItemDTO dto)
        {
            // 2.1) valida existência do orçamento
            if (!await _ctx.Orcamentos.AnyAsync(o => o.OrcamentoID == orcId))
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

            // 2.2) monta entidade e persiste
            var item = new OrcamentoItem
            {
                OrcamentoOrcamentoID = orcId,
                MateriaPrimaID = dto.MateriaPrimaID,
                Quantidade = dto.Quantidade,
                PrecoUnit = dto.PrecoUnit,
                PrazoEntrega = dto.PrazoEntrega
            };
            _ctx.OrcamentosItem.Add(item);
            await _ctx.SaveChangesAsync();

            return CreatedAtAction(
                nameof(ObterOrcamento),
                new { orcId },
                new { OrcamentoItemId = item.OrcamentoItemID }
            );
        }
        #endregion

        #region AtualizarOrcamento
        /// <summary>
        /// 3) Recupera um orçamento e todos os seus itens.
        /// </summary>
        [HttpGet("{orcId:int}")]
        [ProducesResponseType(typeof(OrcamentoDetalheDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObterOrcamento([FromRoute] int orcId)
        {
            var orc = await _ctx.Orcamentos
                .Include(o => o.OrcamentoItems)
                  .ThenInclude(it => it.MateriaPrima)
                .Include(o => o.PedidoCotacaoPedidoCotacao)
                .FirstOrDefaultAsync(o => o.OrcamentoID == orcId);

            if (orc == null)
                return NotFound();

            // 3.1) mapear para DTO de detalhe
            var detalhe = new OrcamentoDetalheDTO
            {
                OrcamentoID = orc.OrcamentoID,
                PedidoCotacaoID = orc.PedidoCotacaoPedidoCotacaoID,
                Data = orc.Data,
                Estado = orc.Estado,
                Itens = orc.OrcamentoItems.Select(i => new OrcamentoItemDetalheDTO
                {
                    OrcamentoItemID = i.OrcamentoItemID,
                    MateriaPrimaID = i.MateriaPrimaID,
                    MateriaPrimaNome = i.MateriaPrima.Nome,
                    Quantidade = i.Quantidade,
                    PrecoUnit = i.PrecoUnit,
                    PrazoEntrega = i.PrazoEntrega ?? 0
                }).ToList()
            };

            return Ok(detalhe);
        }
        #endregion
    }
}