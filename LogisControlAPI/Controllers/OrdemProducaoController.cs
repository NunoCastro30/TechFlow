using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdemProducaoController : ControllerBase
    {
        private readonly LogisControlContext _context;

        public OrdemProducaoController(LogisControlContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdemProducaoDTO>>> GetAll()
        {
            try
            {
                var ordens = await _context.OrdensProducao
                    .Select(o => new OrdemProducaoDTO
                    {
                        OrdemProdId = o.OrdemProdId,
                        Estado = o.Estado,
                        Quantidade = o.Quantidade,
                        DataAbertura = o.DataAbertura,
                        DataConclusao = o.DataConclusao,
                        MaquinaMaquinaId = o.MaquinaMaquinaId,
                        EncomendaClienteEncomendaClienteId = o.EncomendaClienteEncomendaClienteId
                    })
                    .ToListAsync();

                return Ok(ordens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter as ordens de produção: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrdemProducaoDTO>> GetById(int id)
        {
            var ordem = await _context.OrdensProducao.FindAsync(id);
            if (ordem == null)
            {
                return NotFound();
            }
            return Ok(ordem);
        }

        [HttpPost]
        public async Task<ActionResult<OrdemProducaoDTO>> Create([FromBody] OrdemProducaoDTO dto)
        {
            var ordem = new OrdemProducao
            {
                Estado = dto.Estado,
                Quantidade = dto.Quantidade,
                DataAbertura = dto.DataAbertura,
                DataConclusao = dto.DataConclusao,
                MaquinaMaquinaId = dto.MaquinaMaquinaId,
                EncomendaClienteEncomendaClienteId = dto.EncomendaClienteEncomendaClienteId
            };

            _context.OrdensProducao.Add(ordem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = ordem.OrdemProdId }, ordem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrdemProducaoDTO dto)
        {
            var ordem = await _context.OrdensProducao.FindAsync(id);
            if (ordem == null)
            {
                return NotFound();
            }

            ordem.Estado = dto.Estado;
            ordem.Quantidade = dto.Quantidade;
            ordem.DataAbertura = dto.DataAbertura;
            ordem.DataConclusao = dto.DataConclusao;
            ordem.MaquinaMaquinaId = dto.MaquinaMaquinaId;
            ordem.EncomendaClienteEncomendaClienteId = dto.EncomendaClienteEncomendaClienteId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ordem = await _context.OrdensProducao.FindAsync(id);
            if (ordem == null)
            {
                return NotFound();
            }

            _context.OrdensProducao.Remove(ordem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
