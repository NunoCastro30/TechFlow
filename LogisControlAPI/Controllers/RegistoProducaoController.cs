using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RegistoProducaoController : ControllerBase
    {
        private readonly LogisControlContext _context;

        public RegistoProducaoController(LogisControlContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegistoProducaoDTO>>> GetAll()
        {
            try
            {
                var registos = await _context.RegistosProducao
                    .Select(r => new RegistoProducaoDTO
                    {
                        RegistoProducaoId = r.RegistoProducaoId,
                        Estado = r.Estado,
                        DataProducao = r.DataProducao,
                        Observacoes = r.Observacoes,
                        UtilizadorUtilizadorId = r.UtilizadorUtilizadorId,
                        ProdutoProdutoId = r.ProdutoProdutoId,
                        OrdemProducaoOrdemProdId = r.OrdemProducaoOrdemProdId
                    })
                    .ToListAsync();

                return Ok(registos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter os registros de produção: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RegistoProducaoDTO>> GetById(int id)
        {
            var registo = await _context.RegistosProducao.FindAsync(id);
            if (registo == null)
            {
                return NotFound();
            }
            return Ok(registo);
        }

        [HttpPost]
        public async Task<ActionResult<RegistoProducaoDTO>> Create([FromBody] RegistoProducaoDTO dto)
        {
            var registo = new RegistoProducao
            {
                Estado = dto.Estado,
                DataProducao = dto.DataProducao,
                Observacoes = dto.Observacoes,
                UtilizadorUtilizadorId = dto.UtilizadorUtilizadorId,
                ProdutoProdutoId = dto.ProdutoProdutoId,
                OrdemProducaoOrdemProdId = dto.OrdemProducaoOrdemProdId
            };

            _context.RegistosProducao.Add(registo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = registo.RegistoProducaoId }, registo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RegistoProducaoDTO dto)
        {
            var registo = await _context.RegistosProducao.FindAsync(id);
            if (registo == null)
            {
                return NotFound();
            }

            registo.Estado = dto.Estado;
            registo.DataProducao = dto.DataProducao;
            registo.Observacoes = dto.Observacoes;
            registo.UtilizadorUtilizadorId = dto.UtilizadorUtilizadorId;
            registo.ProdutoProdutoId = dto.ProdutoProdutoId;
            registo.OrdemProducaoOrdemProdId = dto.OrdemProducaoOrdemProdId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var registo = await _context.RegistosProducao.FindAsync(id);
            if (registo == null)
            {
                return NotFound();
            }

            _context.RegistosProducao.Remove(registo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
