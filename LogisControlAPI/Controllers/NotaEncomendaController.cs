using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão das Notas de Encomenda.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class NotaEncomendaController : ControllerBase
    {
        private readonly LogisControlContext _context;

        /// <summary>
        /// Construtor do controlador, que injeta o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da base de dados.</param>
        public NotaEncomendaController(LogisControlContext context)
        {
            _context = context;
        }

        #region ObterNotasEncomenda
        /// <summary>
        /// Obtém a lista de todas as notas de encomenda.
        /// </summary>
        /// <returns>Lista de todas as notas de encomenda registadas.</returns>
        /// <response code="200">Retorna a lista de notas de encomenda com sucesso.</response>
        /// <response code="500">Erro interno ao tentar obter as notas de encomenda.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotaEncomendaDTO>>> GetNotasEncomenda()
        {
            try
            {
                var notasEncomenda = await _context.NotasEncomenda
                    .Select(n => new NotaEncomendaDTO
                    {
                       // NotaEncomendaId = n.NotaEncomendaId,
                        DataEmissao = n.DataEmissao,
                        Estado = n.Estado,
                        ValorTotal = n.ValorTotal,
                        OrcamentoOrcamentoId = n.OrcamentoOrcamentoId
                    })
                    .ToListAsync();

                return Ok(notasEncomenda);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter notas de encomenda: {ex.Message}");
            }
        }
        #endregion

        #region ObterNotaEncomendaPorId
        /// <summary>
        /// Obtém uma nota de encomenda pelo seu ID.
        /// </summary>
        /// <param name="id">Identificador da nota de encomenda.</param>
        /// <returns>Dados da nota de encomenda correspondente ao ID.</returns>
        /// <response code="200">Nota de encomenda encontrada com sucesso.</response>
        /// <response code="404">Nota de encomenda não encontrada.</response>
        [HttpGet("GetNotaEncomenda/{id}")]
        public async Task<ActionResult<NotaEncomendaDTO>> GetNotaEncomenda(int id)
        {
            var notaEncomenda = await _context.NotasEncomenda.FindAsync(id);
            if (notaEncomenda == null)
            {
                return NotFound();
            }
            return Ok(new NotaEncomendaDTO
            {
               // NotaEncomendaId = notaEncomenda.NotaEncomendaId,
                DataEmissao = notaEncomenda.DataEmissao,
                Estado = notaEncomenda.Estado,
                ValorTotal = notaEncomenda.ValorTotal,
                OrcamentoOrcamentoId = notaEncomenda.OrcamentoOrcamentoId
            });
        }
        #endregion

        #region CriarNotaEncomenda
        /// <summary>
        /// Cria uma nova nota de encomenda.
        /// </summary>
        /// <param name="notaEncomendaDTO">Dados necessários para criar a nota de encomenda.</param>
        /// <returns>Nota de encomenda criada com sucesso.</returns>
        /// <response code="201">Nota de encomenda criada com sucesso.</response>
        /// <response code="400">Dados inválidos para criação da nota de encomenda.</response>
        /// <response code="500">Erro interno ao criar a nota de encomenda.</response>
        [HttpPost ("CriaNotaEcomenda/")]
        public async Task<ActionResult<NotaEncomendaDTO>> CreateNotaEncomenda([FromBody] NotaEncomendaDTO notaEncomendaDTO)
        {
            try
            {
                var notaEncomenda = new NotaEncomenda
                {
                    DataEmissao = notaEncomendaDTO.DataEmissao,
                    Estado = notaEncomendaDTO.Estado,
                    ValorTotal = notaEncomendaDTO.ValorTotal,
                    OrcamentoOrcamentoId = notaEncomendaDTO.OrcamentoOrcamentoId
                };

                _context.NotasEncomenda.Add(notaEncomenda);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetNotaEncomenda), new { id = notaEncomenda.NotaEncomendaId }, notaEncomenda);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao criar nota de encomenda: {ex.Message}");
            }
        }
        #endregion

        #region AtualizarNotaEncomenda
        /// <summary>
        /// Atualiza os dados de uma nota de encomenda existente.
        /// </summary>
        /// <param name="id">ID da nota de encomenda a ser atualizada.</param>
        /// <param name="notaEncomendaDTO">Dados atualizados da nota de encomenda.</param>
        /// <returns>Resposta de sucesso ou erro.</returns>
        /// <response code="204">Nota de encomenda atualizada com sucesso.</response>
        /// <response code="404">Nota de encomenda não encontrada.</response>
        /// <response code="500">Erro interno ao tentar atualizar a nota de encomenda.</response>
        [HttpPut("UpdateNotaEncomenda/{id}")]
        public async Task<IActionResult> UpdateNotaEncomenda(int id, [FromBody] NotaEncomendaDTO notaEncomendaDTO)
        {
            try
            {
                var notaEncomenda = await _context.NotasEncomenda.FindAsync(id);
                if (notaEncomenda == null)
                {
                    return NotFound();
                }

                notaEncomenda.DataEmissao = notaEncomendaDTO.DataEmissao;
                notaEncomenda.Estado = notaEncomendaDTO.Estado;
                notaEncomenda.ValorTotal = notaEncomendaDTO.ValorTotal;
                notaEncomenda.OrcamentoOrcamentoId = notaEncomendaDTO.OrcamentoOrcamentoId;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao atualizar a nota de encomenda: {ex.Message}");
            }
        }
        #endregion

        #region DeletarNotaEncomenda
        /// <summary>
        /// Remove uma nota de encomenda existente.
        /// </summary>
        /// <param name="id">ID da nota de encomenda a ser removida.</param>
        /// <returns>Resposta de sucesso ou erro.</returns>
        /// <response code="204">Nota de encomenda removida com sucesso.</response>
        /// <response code="404">Nota de encomenda não encontrada.</response>
        /// <response code="500">Erro interno ao tentar remover a nota de encomenda.</response>
        [HttpDelete("DeleteNotaEncomenda/{id}")]
        public async Task<IActionResult> DeleteNotaEncomenda(int id)
        {
            try
            {
                var notaEncomenda = await _context.NotasEncomenda.FindAsync(id);
                if (notaEncomenda == null)
                {
                    return NotFound();
                }

                _context.NotasEncomenda.Remove(notaEncomenda);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao remover a nota de encomenda: {ex.Message}");
            }
        }
        #endregion
    }
}
