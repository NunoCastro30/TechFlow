using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using LogisControlAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Controllers
{
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

        /// <summary>
        /// 2) Fornecedor adiciona linhas ao orçamento.
        /// </summary>
        [HttpPost("{orcId:int}/itens")]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AdicionarItem(
    [FromRoute] int orcId,
    [FromBody] CriarOrcamentoItemDTO dto)
        {
            if (!await _ctx.Orcamentos.AnyAsync(o => o.OrcamentoID == orcId))
                return NotFound("Orçamento não encontrado.");

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

            return CreatedAtAction(nameof(ObterOrcamento), new { orcId }, new { item.OrcamentoItemID });
        }

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

        /// <summary>
        /// Aceita um orçamento e recusa todos os outros do mesmo pedido de cotação.
        /// </summary>
        [HttpPost("{orcId:int}/aceitar")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AceitarOrcamento([FromRoute] int orcId)
        {
            // 1) Carrega orçamento e pedido associado
            var orcamento = await _ctx.Orcamentos
                .Include(o => o.PedidoCotacaoPedidoCotacao)
                .Where(o => o.OrcamentoID == orcId)
                .FirstOrDefaultAsync();

            if (orcamento == null)
                return NotFound("Orçamento não encontrado.");

            var pedidoCotacaoId = orcamento.PedidoCotacaoPedidoCotacaoID;

            // 2) Vai buscar todos os orçamentos do mesmo pedido
            var todosOrcamentos = await _ctx.Orcamentos
                .Where(o => o.PedidoCotacaoPedidoCotacaoID == pedidoCotacaoId)
                .ToListAsync();

            // 3) Marca todos como recusado, exceto o atual
            foreach (var o in todosOrcamentos)
                o.Estado = (o.OrcamentoID == orcId ? "Aceite" : "Recusado");

            // 4) Atualiza estado do PedidoCotacao
            var pedidoCotacao = orcamento.PedidoCotacaoPedidoCotacao;
            pedidoCotacao.Estado = "Finalizado";

            // 5) Guarda alterações
            await _ctx.SaveChangesAsync();

            return Ok("Orçamento aceite com sucesso.");
        }
    }
}