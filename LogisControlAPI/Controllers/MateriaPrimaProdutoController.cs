using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriaPrimaProdutoController : ControllerBase
    {
        private readonly LogisControlContext _context;

        public MateriaPrimaProdutoController(LogisControlContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MateriaPrimaProdutoDTO>>> GetAll()
        {
            try
            {
                var items = await _context.MateriaPrimaProdutos
                    .Select(mpp => new MateriaPrimaProdutoDTO
                    {
                        MateriaPrimaProdutoId = mpp.MateriaPrimaProdutoId,
                        QuantidadeNec = mpp.QuantidadeNec,
                        MateriaPrimaMateriaPrimaId = mpp.MateriaPrimaMateriaPrimaId,
                        ProdutoProdutoId = mpp.ProdutoProdutoId
                    })
                    .ToListAsync();

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter os registros: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MateriaPrimaProdutoDTO>> GetById(int id)
        {
            var item = await _context.MateriaPrimaProdutos.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<MateriaPrimaProdutoDTO>> Create([FromBody] MateriaPrimaProdutoDTO dto)
        {
            var entity = new MateriaPrimaProduto
            {
                QuantidadeNec = dto.QuantidadeNec,
                MateriaPrimaMateriaPrimaId = dto.MateriaPrimaMateriaPrimaId,
                ProdutoProdutoId = dto.ProdutoProdutoId
            };

            _context.MateriaPrimaProdutos.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.MateriaPrimaProdutoId }, entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MateriaPrimaProdutoDTO dto)
        {
            var entity = await _context.MateriaPrimaProdutos.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.QuantidadeNec = dto.QuantidadeNec;
            entity.MateriaPrimaMateriaPrimaId = dto.MateriaPrimaMateriaPrimaId;
            entity.ProdutoProdutoId = dto.ProdutoProdutoId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.MateriaPrimaProdutos.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            _context.MateriaPrimaProdutos.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
