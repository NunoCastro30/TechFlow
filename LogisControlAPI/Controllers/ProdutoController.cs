using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão dos produtos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly LogisControlContext _context;

        /// <summary>
        /// Construtor do controlador que injeta o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da base de dados.</param>
        public ProdutoController(LogisControlContext context)
        {
            _context = context;
        }

        #region ListarProdutos
        /// <summary>
        /// Lista todos os produtos.
        /// </summary>
        /// <returns>Lista de produtos.</returns>
        /// <response code="200">Lista obtida com sucesso.</response>
        /// <response code="500">Erro interno ao obter produtos.</response>
        [HttpGet("ListarProdutos")]
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
        #endregion

        #region ObterProdutoPorId
        /// <summary>
        /// Obtém um produto pelo ID.
        /// </summary>
        /// <param name="id">ID do produto.</param>
        /// <returns>Produto correspondente ao ID.</returns>
        /// <response code="200">Produto encontrado.</response>
        /// <response code="404">Produto não encontrado.</response>
        [HttpGet("ObterProdutoPorId/{id}")]
        public async Task<ActionResult<ProdutoDTO>> GetById(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            return Ok(produto);
        }
        #endregion

        #region CriarProduto
        /// <summary>
        /// Cria um novo produto.
        /// </summary>
        /// <param name="dto">Dados do produto a ser criado.</param>
        /// <returns>Produto criado.</returns>
        /// <response code="201">Produto criado com sucesso.</response>
        [HttpPost("CriarProduto")]
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
        #endregion

        #region AtualizarProduto
        /// <summary>
        /// Atualiza um produto existente.
        /// </summary>
        /// <param name="id">ID do produto a ser atualizado.</param>
        /// <param name="dto">Novos dados do produto.</param>
        /// <returns>Status da atualização.</returns>
        /// <response code="204">Produto atualizado com sucesso.</response>
        /// <response code="404">Produto não encontrado.</response>
        [HttpPut("AtualizarProduto/{id}")]
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
        #endregion

        #region DeletarProduto
        /// <summary>
        /// Exclui um produto pelo ID.
        /// </summary>
        /// <param name="id">ID do produto a ser excluído.</param>
        /// <returns>Status da exclusão.</returns>
        /// <response code="204">Produto excluído com sucesso.</response>
        /// <response code="404">Produto não encontrado.</response>
        [HttpDelete("ApagarProduto/{id}")]
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
        #endregion
    }
}
