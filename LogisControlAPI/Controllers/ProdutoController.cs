using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly LogisControlContext _context;

        public ProdutoController(LogisControlContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetAll()
        {
            try
            {
                var produtos = await _context.Produtos
                    .Select(p => new ProdutoDTO
                    {
                        ProdutoId = p.ProdutoId,
                        Nome = p.Nome,
                        Quantidade = p.Quantidade,
                        Descricao = p.Descricao,
                        CodInterno = p.CodInterno,
                        Preco = p.Preco,
                        OrdemProducaoOrdemProdId = p.OrdemProducaoOrdemProdId,
                        EncomendaItensEncomendaItensId = p.EncomendaItensEncomendaItensId
                    })
                    .ToListAsync();

                return Ok(produtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter os produtos: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoDTO>> GetById(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            return Ok(produto);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoDTO>> Create([FromBody] ProdutoDTO dto)
        {
            var produto = new Produto
            {
                Nome = dto.Nome,
                Quantidade = dto.Quantidade,
                Descricao = dto.Descricao,
                CodInterno = dto.CodInterno,
                Preco = dto.Preco,
                OrdemProducaoOrdemProdId = dto.OrdemProducaoOrdemProdId,
                EncomendaItensEncomendaItensId = dto.EncomendaItensEncomendaItensId
            };

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = produto.ProdutoId }, produto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProdutoDTO dto)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            produto.Nome = dto.Nome;
            produto.Quantidade = dto.Quantidade;
            produto.Descricao = dto.Descricao;
            produto.CodInterno = dto.CodInterno;
            produto.Preco = dto.Preco;
            produto.OrdemProducaoOrdemProdId = dto.OrdemProducaoOrdemProdId;
            produto.EncomendaItensEncomendaItensId = dto.EncomendaItensEncomendaItensId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
