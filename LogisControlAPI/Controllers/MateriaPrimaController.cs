using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogisControlAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão das matérias-primas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MateriaPrimaController : ControllerBase
    {
        private readonly LogisControlContext _context;

        /// <summary>
        /// Construtor do controlador que injeta o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da base de dados.</param>
        public MateriaPrimaController(LogisControlContext context)
        {
            _context = context;
        }

        #region ListarMateriasPrimas
        /// <summary>
        /// Lista todas as matérias-primas.
        /// </summary>
        /// <returns>Lista de matérias-primas.</returns>
        /// <response code="200">Lista obtida com sucesso.</response>
        /// <response code="500">Erro interno ao obter matérias-primas.</response>
        [HttpGet("ListarMateriasPrimas")]
        public async Task<ActionResult<IEnumerable<MateriaPrimaDTO>>> GetMateriasPrimas()
        {
            try
            {
                var materiasPrimas = await _context.MateriasPrimas
                    .Select(m => new MateriaPrimaDTO
                    {
                        MateriaPrimaId = m.MateriaPrimaId,
                        Nome = m.Nome,
                        Quantidade = m.Quantidade,
                        Descricao = m.Descricao,
                        Categoria = m.Categoria,
                        CodInterno = m.CodInterno,
                        Preco = m.Preco
                    })
                    .ToListAsync();

                return Ok(materiasPrimas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter matérias-primas: {ex.Message}");
            }
        }
        #endregion

        #region ObterMateriaPrimaPorId
        /// <summary>
        /// Obtém uma matéria-prima pelo ID.
        /// </summary>
        /// <param name="id">ID da matéria-prima.</param>
        /// <returns>Dados da matéria-prima.</returns>
        /// <response code="200">Matéria-prima encontrada.</response>
        /// <response code="404">Matéria-prima não encontrada.</response>
        [HttpGet("ObterMateriaPrimaPorId/{id}")]
        public async Task<ActionResult<MateriaPrimaDTO>> GetMateriaPrima(int id)
        {
            var materiaPrima = await _context.MateriasPrimas.FindAsync(id);
            if (materiaPrima == null)
            {
                return NotFound();
            }
            return Ok(materiaPrima);
        }
        #endregion

        #region CriarMateriaPrima
        /// <summary>
        /// Cria uma nova matéria-prima.
        /// </summary>
        /// <param name="materiaPrimaDTO">Dados da nova matéria-prima.</param>
        /// <returns>Matéria-prima criada.</returns>
        /// <response code="201">Matéria-prima criada com sucesso.</response>
        [HttpPost("CriarMateriaPrima")]
        public async Task<ActionResult<MateriaPrimaDTO>> CreateMateriaPrima([FromBody] MateriaPrimaDTO materiaPrimaDTO)
        {
            var materiaPrima = new MateriaPrima
            {
                Nome = materiaPrimaDTO.Nome,
                Quantidade = materiaPrimaDTO.Quantidade,
                Descricao = materiaPrimaDTO.Descricao,
                Categoria = materiaPrimaDTO.Categoria,
                CodInterno = materiaPrimaDTO.CodInterno,
                Preco = materiaPrimaDTO.Preco
            };

            _context.MateriasPrimas.Add(materiaPrima);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMateriaPrima), new { id = materiaPrima.MateriaPrimaId }, materiaPrima);
        }
        #endregion

        #region AtualizarMateriaPrima
        /// <summary>
        /// Atualiza os dados de uma matéria-prima existente.
        /// </summary>
        /// <param name="id">ID da matéria-prima a ser atualizada.</param>
        /// <param name="materiaPrimaDTO">Novos dados da matéria-prima.</param>
        /// <returns>Resultado da atualização.</returns>
        /// <response code="204">Matéria-prima atualizada com sucesso.</response>
        /// <response code="404">Matéria-prima não encontrada.</response>
        [HttpPut("AtualizarMateriaPrima/{id}")]
        public async Task<IActionResult> UpdateMateriaPrima(int id, [FromBody] MateriaPrimaDTO materiaPrimaDTO)
        {
            var materiaPrima = await _context.MateriasPrimas.FindAsync(id);
            if (materiaPrima == null)
            {
                return NotFound();
            }

            materiaPrima.Nome = materiaPrimaDTO.Nome;
            materiaPrima.Quantidade = materiaPrimaDTO.Quantidade;
            materiaPrima.Descricao = materiaPrimaDTO.Descricao;
            materiaPrima.Categoria = materiaPrimaDTO.Categoria;
            materiaPrima.CodInterno = materiaPrimaDTO.CodInterno;
            materiaPrima.Preco = materiaPrimaDTO.Preco;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region DeletarMateriaPrima
        /// <summary>
        /// Exclui uma matéria-prima pelo ID.
        /// </summary>
        /// <param name="id">ID da matéria-prima a ser excluída.</param>
        /// <returns>Resultado da exclusão.</returns>
        /// <response code="204">Matéria-prima excluída com sucesso.</response>
        /// <response code="404">Matéria-prima não encontrada.</response>
        [HttpDelete("ApagarMateriaPrima/{id}")]
        public async Task<IActionResult> DeleteMateriaPrima(int id)
        {
            var materiaPrima = await _context.MateriasPrimas.FindAsync(id);
            if (materiaPrima == null)
            {
                return NotFound();
            }

            _context.MateriasPrimas.Remove(materiaPrima);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        #endregion
    }
}