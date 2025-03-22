using Microsoft.AspNetCore.Mvc;
using LogisControlAPI.Models;
using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão dos registos de manutenção.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RegistoManutencaoController : ControllerBase
    {
        private readonly LogisControlContext _context;

        /// <summary>
        /// Construtor do controlador que injeta o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da base de dados.</param>
        public RegistoManutencaoController(LogisControlContext context)
        {
            _context = context;
        }

        #region ObterRegistos
        /// <summary>
        /// Obtém a lista de todos os registos de manutenção.
        /// </summary>
        /// <returns>Lista de registos de manutenção.</returns>
        /// <response code="200">Retorna a lista de registos com sucesso.</response>
        /// <response code="500">Erro interno ao tentar obter os registos.</response>
        [HttpGet("ObterRegistos")]
        public async Task<ActionResult<IEnumerable<RegistoManutencaoDTO>>> GetRegistos()
        {
            try
            {
                var registos = await _context.RegistoManutencaos
                    .Select(r => new RegistoManutencaoDTO
                    {
                        RegistoManutencaoId = r.RegistoManutencaoId,
                        Descricao = r.Descricao,
                        Estado = r.Estado,
                        PedidoManutencaoPedidoManutId = r.PedidoManutencaoPedidoManutId,
                        UtilizadorUtilizadorId = r.UtilizadorUtilizadorId,
                        AssistenciaExternaAssistenteId = r.AssistenciaExternaAssistenteId
                    })
                    .ToListAsync();

                return Ok(registos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter registos de manutenção: {ex.Message}");
            }
        }
        #endregion

        #region ObterRegistoPorId
        /// <summary>
        /// Obtém um registo de manutenção pelo seu ID.
        /// </summary>
        /// <param name="id">Identificador único do registo.</param>
        /// <returns>Dados do registo correspondente.</returns>
        /// <response code="200">Registo encontrado com sucesso.</response>
        /// <response code="404">Registo não encontrado.</response>
        /// <response code="500">Erro interno ao procurar o registo.</response>
        [HttpGet("ObterRegisto/{id}")]
        public async Task<ActionResult<RegistoManutencaoDTO>> GetRegistoPorId(int id)
        {
            try
            {
                var registo = await _context.RegistoManutencaos
                    .Where(r => r.RegistoManutencaoId == id)
                    .Select(r => new RegistoManutencaoDTO
                    {
                        RegistoManutencaoId = r.RegistoManutencaoId,
                        Descricao = r.Descricao,
                        Estado = r.Estado,
                        PedidoManutencaoPedidoManutId = r.PedidoManutencaoPedidoManutId,
                        UtilizadorUtilizadorId = r.UtilizadorUtilizadorId,
                        AssistenciaExternaAssistenteId = r.AssistenciaExternaAssistenteId
                    })
                    .FirstOrDefaultAsync();

                if (registo == null)
                    return NotFound("Registo de manutenção não encontrado.");

                return Ok(registo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter registo: {ex.Message}");
            }
        }
        #endregion

        #region CriarRegisto
        /// <summary>
        /// Cria um novo registo de manutenção.
        /// </summary>
        /// <param name="novoRegistoDto">Dados para criação do registo.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="201">Registo criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="500">Erro interno ao criar o registo.</response>
        [HttpPost("CriarRegisto")]
        public async Task<ActionResult> CriarRegisto([FromBody] RegistoManutencaoDTO novoRegistoDto)
        {
            try
            {
                var novoRegisto = new RegistoManutencao
                {
                    Descricao = novoRegistoDto.Descricao,
                    Estado = novoRegistoDto.Estado,
                    PedidoManutencaoPedidoManutId = novoRegistoDto.PedidoManutencaoPedidoManutId,
                    UtilizadorUtilizadorId = novoRegistoDto.UtilizadorUtilizadorId,
                    AssistenciaExternaAssistenteId = novoRegistoDto.AssistenciaExternaAssistenteId
                };

                _context.RegistoManutencaos.Add(novoRegisto);
                await _context.SaveChangesAsync();

                return StatusCode(201, "Registo de manutenção criado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao criar registo de manutenção: {ex.Message}");
            }
        }
        #endregion

        #region AtualizarRegisto
        /// <summary>
        /// Atualiza os dados de um registo de manutenção existente.
        /// </summary>
        /// <param name="registoId">ID do registo a atualizar.</param>
        /// <param name="registoAtualizado">Dados atualizados do registo.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Registo atualizado com sucesso.</response>
        /// <response code="404">Registo não encontrado.</response>
        /// <response code="500">Erro interno ao tentar atualizar o registo.</response>
        [HttpPut("AtualizarRegisto/{registoId}")]
        public async Task<IActionResult> AtualizarRegisto(int registoId, [FromBody] RegistoManutencaoDTO registoAtualizado)
        {
            try
            {
                var registo = await _context.RegistoManutencaos.FindAsync(registoId);

                if (registo == null)
                    return NotFound("Registo de manutenção não encontrado.");

                // Atualizar os campos
                registo.Descricao = registoAtualizado.Descricao;
                registo.Estado = registoAtualizado.Estado;
                registo.PedidoManutencaoPedidoManutId = registoAtualizado.PedidoManutencaoPedidoManutId;
                registo.UtilizadorUtilizadorId = registoAtualizado.UtilizadorUtilizadorId;
                registo.AssistenciaExternaAssistenteId = registoAtualizado.AssistenciaExternaAssistenteId;

                await _context.SaveChangesAsync();

                return Ok("Registo de manutenção atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar registo de manutenção: {ex.Message}");
            }
        }
        #endregion
    }
}
