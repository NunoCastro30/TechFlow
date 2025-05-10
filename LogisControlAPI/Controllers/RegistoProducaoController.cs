using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using LogisControlAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão dos registos de produção.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RegistoProducaoController : ControllerBase
    {
        private readonly LogisControlContext _context;
        private readonly ProducaoService _producaoService;

        /// <summary>
        /// Construtor do controlador que injeta o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da base de dados.</param>
        public RegistoProducaoController(LogisControlContext context, ProducaoService producaoService)
        {
            _context = context;
            _producaoService = producaoService;
        }

        #region ListarRegistos
        /// <summary>
        /// Obtém todos os registos de produção.
        /// </summary>
        /// <returns>Lista de registos de produção.</returns>
        /// <response code="200">Lista obtida com sucesso.</response>
        /// <response code="500">Erro ao obter os registos de produção.</response>
        [HttpGet("ListarRegistosProducao")]
        public async Task<ActionResult<IEnumerable<RegistoProducaoDTO>>> GetAll()
        {
            try
            {
                var registos = await _context.RegistosProducao
                    .Include(r => r.ProdutoProduto)
                    .Include(r => r.UtilizadorUtilizador)
                    .Select(r => new RegistoProducaoDTO
                    {
                        RegistoProducaoId = r.RegistoProducaoId,
                        Estado = r.Estado,
                        DataProducao = r.DataProducao,
                        Observacoes = r.Observacoes,
                        NomeProduto = r.ProdutoProduto.Nome,
                        NomeUtilizador = r.UtilizadorUtilizador.PrimeiroNome + " " + r.UtilizadorUtilizador.Sobrenome,
                        OrdemProducaoOrdemProdId = r.OrdemProducaoOrdemProdId
                    })
                    .ToListAsync();

                return Ok(registos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter os registos de produção: {ex.Message}");
            }
        }
        #endregion

        #region ObterPorId
        /// <summary>
        /// Obtém um registo de produção pelo ID.
        /// </summary>
        /// <param name="id">ID do registo de produção.</param>
        /// <returns>Dados do registo de produção.</returns>
        /// <response code="200">Registo encontrado.</response>
        /// <response code="404">Registo não encontrado.</response>
        [HttpGet("ObterRegistoProducaoPorId/{id}")]
        public async Task<ActionResult<RegistoProducaoDTO>> GetById(int id)
        {
            var registo = await _context.RegistosProducao
                .Include(r => r.ProdutoProduto)
                .Include(r => r.UtilizadorUtilizador)
                .FirstOrDefaultAsync(r => r.RegistoProducaoId == id);

            if (registo == null)
            {
                return NotFound();
            }

            var registoDto = new RegistoProducaoDTO
            {
                RegistoProducaoId = registo.RegistoProducaoId,
                Estado = registo.Estado,
                DataProducao = registo.DataProducao,
                Observacoes = registo.Observacoes,
                NomeProduto = registo.ProdutoProduto.Nome,
                NomeUtilizador = registo.UtilizadorUtilizador.PrimeiroNome + " " + registo.UtilizadorUtilizador.Sobrenome,
                OrdemProducaoOrdemProdId = registo.OrdemProducaoOrdemProdId
            };

            return Ok(registoDto);
        }
        #endregion

        #region CriarRegisto
        /// <summary>
        /// Cria um novo registo de produção.
        /// </summary>
        /// <param name="dto">Dados do novo registo de produção.</param>
        /// <returns>Registo criado.</returns>
        /// <response code="201">Registo criado com sucesso.</response>
        [HttpPost("CriarRegistoProducao")]
        public async Task<ActionResult<RegistoProducaoDTO>> Create([FromBody] RegistoProducaoCreateDTO dto)
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

            // Carregar dados adicionais para montar DTO de leitura
            var produto = await _context.Produtos.FindAsync(dto.ProdutoProdutoId);
            var utilizador = await _context.Utilizadores.FindAsync(dto.UtilizadorUtilizadorId);

            var registoDTO = new RegistoProducaoDTO
            {
                RegistoProducaoId = registo.RegistoProducaoId,
                Estado = registo.Estado,
                DataProducao = registo.DataProducao,
                Observacoes = registo.Observacoes,
                NomeProduto = produto?.Nome,
                NomeUtilizador = utilizador != null ? $"{utilizador.PrimeiroNome} {utilizador.Sobrenome}" : string.Empty,
                OrdemProducaoOrdemProdId = registo.OrdemProducaoOrdemProdId
            };

            return CreatedAtAction(nameof(GetById), new { id = registo.RegistoProducaoId }, registoDTO);
        }
        #endregion

        #region AtualizarRegisto
        /// <summary>
        /// Atualiza os dados de um registo de produção.
        /// </summary>
        /// <param name="id">ID do registo a ser atualizado.</param>
        /// <param name="dto">Novos dados do registo de produção.</param>
        /// <returns>Sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Registo atualizado com sucesso.</response>
        /// <response code="404">Registo não encontrado.</response>
        [HttpPut("AtualizarRegistoProducao/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RegistoProducaoCreateDTO dto)
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
        #endregion

        #region AtualizarEstadoEObservacoesRegistoProducao
        /// <summary>
        /// Atualiza o estado e as observações de um registo de produção.
        /// </summary>
        /// <param name="id">ID do registo a ser atualizado.</param>
        /// <param name="dto">Novos dados do registo de produção, contendo o estado e as observações.</param>
        /// <returns>Sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Registo atualizado com sucesso.</response>
        /// <response code="404">Registo não encontrado.</response>
        [HttpPatch("AtualizarEstadoEObservacoesRegistoProducao/{id}")]
        public async Task<IActionResult> UpdateEstadoEObservacoes(int id, [FromBody] RegistoProducaoUpdateEstadoObservacoesDTO dto)
        {
            try
            {
                await _producaoService.AtualizarEstadoEObservacoesAsync(id, dto.Estado, dto.Observacoes);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        #endregion

        #region DeletarRegisto
        /// <summary>
        /// Exclui um registo de produção pelo ID.
        /// </summary>
        /// <param name="id">ID do registo a ser excluído.</param>
        /// <returns>Sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Registo excluído com sucesso.</response>
        /// <response code="404">Registo não encontrado.</response>
        [HttpDelete("ApagarRegistoProducao/{id}")]
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
        #endregion
    }
}
