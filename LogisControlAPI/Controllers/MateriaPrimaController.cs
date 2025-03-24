using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MateriaPrimaController : ControllerBase
    {
        private readonly LogisControlContext _context;

        public MateriaPrimaController(LogisControlContext context)
        {
            _context = context;
        }

        [HttpGet]
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

        [HttpGet("{id}")]
        public async Task<ActionResult<MateriaPrimaDTO>> GetMateriaPrima(int id)
        {
            var materiaPrima = await _context.MateriasPrimas.FindAsync(id);
            if (materiaPrima == null)
            {
                return NotFound();
            }
            return Ok(materiaPrima);
        }

        [HttpPost]
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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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
    }
}
