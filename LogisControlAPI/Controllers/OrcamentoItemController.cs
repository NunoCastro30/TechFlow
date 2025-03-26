using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão dos itens de orçamentos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrcamentoItemController : ControllerBase
    {
        private readonly LogisControlContext _context;

        /// <summary>
        /// Construtor do controlador que injeta o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da base de dados.</param>
        public OrcamentoItemController(LogisControlContext context)
        {
            _context = context;
        }

        #region ObterOrcamentoItens
        /// <summary>
        /// Obtém a lista de todos os itens de orçamentos registados.
        /// </summary>
        /// <returns>Lista de itens de orçamentos existentes.</returns>
        /// <response code="200">Retorna a lista de itens de orçamentos com sucesso.</response>
        /// <response code="500">Erro interno ao tentar obter os itens do orçamento.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrcamentoItemDTO>>> GetOrcamentoItens()
        {
            try
            {
                var orcamentoItens = await _context.OrcamentosItem
                    .Select(o => new OrcamentoItemDTO
                    {
                        OrcamentoItemId = o.OrcamentoItemId,
                        Quantidade = o.Quantidade,
                        PrecoUnit = o.PrecoUnit,
                        PrazoEntrega = o.PrazoEntrega,
                        OrcamentoOrcamentoId = o.OrcamentoOrcamentoId,
                        MateriaPrimaMateriaPrimaId = o.MateriaPrimaMateriaPrimaId
                    })
                    .ToListAsync();

                return Ok(orcamentoItens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter itens do orçamento: {ex.Message}");
            }
        }
        #endregion

        #region ObterOrcamentoItemPorId
        /// <summary>
        /// Obtém um item de orçamento pelo seu ID.
        /// </summary>
        /// <param name="id">Identificador do item de orçamento.</param>
        /// <returns>Dados do item de orçamento correspondente.</returns>
        /// <response code="200">Item de orçamento encontrado com sucesso.</response>
        /// <response code="404">Item de orçamento não encontrado.</response>
        [HttpGet("GetOrcamentoItem/{id}")]
        public async Task<ActionResult<OrcamentoItemDTO>> GetOrcamentoItem(int id)
        {
            var orcamentoItem = await _context.OrcamentosItem.FindAsync(id);
            if (orcamentoItem == null)
            {
                return NotFound();
            }
            return Ok(orcamentoItem);
        }
        #endregion

        #region CriarOrcamentoItem
        /// <summary>
        /// Cria um novo item de orçamento.
        /// </summary>
        /// <param name="orcamentoItemDTO">Dados para a criação do item de orçamento.</param>
        /// <returns>Item de orçamento criado com sucesso.</returns>
        /// <response code="201">Item de orçamento criado com sucesso.</response>
        /// <response code="400">Dados inválidos para o item de orçamento.</response>
        /// <response code="500">Erro interno ao criar o item de orçamento.</response>
        [HttpPost]
        public async Task<ActionResult<OrcamentoItemDTO>> CreateOrcamentoItem([FromBody] OrcamentoItemDTO orcamentoItemDTO)
        {
            var orcamentoItem = new OrcamentoItem
            {
                Quantidade = orcamentoItemDTO.Quantidade,
                PrecoUnit = orcamentoItemDTO.PrecoUnit,
                PrazoEntrega = orcamentoItemDTO.PrazoEntrega,
                OrcamentoOrcamentoId = orcamentoItemDTO.OrcamentoOrcamentoId,
                MateriaPrimaMateriaPrimaId = orcamentoItemDTO.MateriaPrimaMateriaPrimaId
            };

            _context.OrcamentosItem.Add(orcamentoItem);
            await _context.SaveChangesAsync();

            // Retorna o novo item criado com a rota para consulta
            return CreatedAtAction(nameof(GetOrcamentoItem), new { id = orcamentoItem.OrcamentoItemId }, orcamentoItem);
        }
        #endregion

        #region AtualizarOrcamentoItem
        /// <summary>
        /// Atualiza os dados de um item de orçamento existente.
        /// </summary>
        /// <param name="id">ID do item de orçamento a atualizar.</param>
        /// <param name="orcamentoItemDTO">Dados atualizados do item de orçamento.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="204">Item de orçamento atualizado com sucesso.</response>
        /// <response code="404">Item de orçamento não encontrado.</response>
        /// <response code="500">Erro interno ao tentar atualizar o item de orçamento.</response>
        [HttpPut("UpdateOrcamentoItem/{id}")]
        public async Task<IActionResult> UpdateOrcamentoItem(int id, [FromBody] OrcamentoItemDTO orcamentoItemDTO)
        {
            var orcamentoItem = await _context.OrcamentosItem.FindAsync(id);
            if (orcamentoItem == null)
            {
                return NotFound();
            }

            // Atualiza os dados do item de orçamento
            orcamentoItem.Quantidade = orcamentoItemDTO.Quantidade;
            orcamentoItem.PrecoUnit = orcamentoItemDTO.PrecoUnit;
            orcamentoItem.PrazoEntrega = orcamentoItemDTO.PrazoEntrega;
            orcamentoItem.OrcamentoOrcamentoId = orcamentoItemDTO.OrcamentoOrcamentoId;
            orcamentoItem.MateriaPrimaMateriaPrimaId = orcamentoItemDTO.MateriaPrimaMateriaPrimaId;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region DeletarOrcamentoItem
        /// <summary>
        /// Remove um item de orçamento existente.
        /// </summary>
        /// <param name="id">ID do item de orçamento a remover.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="204">Item de orçamento removido com sucesso.</response>
        /// <response code="404">Item de orçamento não encontrado.</response>
        /// <response code="500">Erro interno ao tentar remover o item de orçamento.</response>
        [HttpDelete("DeleteOrcamentoItem/{id}")]
        public async Task<IActionResult> DeleteOrcamentoItem(int id)
        {
            var orcamentoItem = await _context.OrcamentosItem.FindAsync(id);
            if (orcamentoItem == null)
            {
                return NotFound();
            }

            _context.OrcamentosItem.Remove(orcamentoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        #endregion
    }
}
