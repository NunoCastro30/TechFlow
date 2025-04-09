using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;

namespace LogisControlAPI.Controllers
{
    /// <summary>
    /// Controlador responsável por atualizar a quantidade de matérias-primas utilizadas numa ordem de produção.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProdMateriaisController : ControllerBase
    {
        private readonly LogisControlContext _context;

        public ProdMateriaisController(LogisControlContext context)
        {
            _context = context;
        }

        #region AtualizarQuantidade
        /// <summary>
        /// Atualiza a quantidade utilizada de uma matéria-prima numa ordem de produção.
        /// </summary>
        /// <param name="id">ID do registo ProdMateriais.</param>
        /// <param name="dto">Nova quantidade a atualizar.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Quantidade atualizada com sucesso.</response>
        /// <response code="404">Registo não encontrado.</response>
        /// <response code="500">Erro interno ao atualizar o registo.</response>
        [HttpPut("AtualizarQuantidade/{id}")]
        public async Task<IActionResult> AtualizarQuantidade(int id, [FromBody] ProdMaterialDTO dto)
        {
            try
            {
                var registo = await _context.ProdMateriais.FindAsync(id);
                if (registo == null)
                    return NotFound("Registo de material não encontrado.");

                registo.QuantidadeUtilizada = dto.QuantidadeUtilizada;

                await _context.SaveChangesAsync();
                return Ok("Quantidade atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar a quantidade: {ex.Message}");
            }
        }
        #endregion

    }
}