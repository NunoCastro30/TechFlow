﻿using Microsoft.AspNetCore.Mvc;
using LogisControlAPI.Models;
using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão das assistências externas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AssistenciaExternaController : ControllerBase
    {
        private readonly LogisControlContext _context;

        /// <summary>
        /// Construtor do controlador que injeta o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da base de dados.</param>
        public AssistenciaExternaController(LogisControlContext context)
        {
            _context = context;
        }

        #region ObterAssistencias
        /// <summary>
        /// Obtém a lista de todas as assistências externas registadas.
        /// </summary>
        /// <returns>Lista de assistências externas.</returns>
        /// <response code="200">Retorna a lista com sucesso.</response>
        /// <response code="500">Erro interno ao tentar obter as assistências.</response>
        [HttpGet("ObterAssistencias")]
        public async Task<ActionResult<IEnumerable<AssistenciaExternaDTO>>> GetAssistencias()
        {
            try
            {
                var assistencias = await _context.AssistenciaExternas
                    .Select(a => new AssistenciaExternaDTO
                    {
                        AssistenteId = a.AssistenteId,
                        Nome = a.Nome,
                        Nif = a.Nif,
                        Morada = a.Morada,
                        Telefone = a.Telefone
                    })
                    .ToListAsync();

                return Ok(assistencias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter assistências externas: {ex.Message}");
            }
        }
        #endregion

        #region ObterAssistenciaPorId
        /// <summary>
        /// Obtém uma assistência externa pelo seu ID.
        /// </summary>
        /// <param name="id">Identificador único da assistência.</param>
        /// <returns>Dados da assistência correspondente.</returns>
        /// <response code="200">Assistência encontrada com sucesso.</response>
        /// <response code="404">Assistência não encontrada.</response>
        /// <response code="500">Erro interno ao procurar a assistência.</response>
        [HttpGet("ObterAssistencia/{id}")]
        public async Task<ActionResult<AssistenciaExternaDTO>> GetAssistenciaPorId(int id)
        {
            try
            {
                var assistencia = await _context.AssistenciaExternas
                    .Where(a => a.AssistenteId == id)
                    .Select(a => new AssistenciaExternaDTO
                    {
                        AssistenteId = a.AssistenteId,
                        Nome = a.Nome,
                        Nif = a.Nif,
                        Morada = a.Morada,
                        Telefone = a.Telefone
                    })
                    .FirstOrDefaultAsync();

                if (assistencia == null)
                    return NotFound("Assistência externa não encontrada.");

                return Ok(assistencia);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter assistência externa: {ex.Message}");
            }
        }
        #endregion

        #region CriarAssistencia
        /// <summary>
        /// Cria uma nova assistência externa.
        /// </summary>
        /// <param name="novaAssistenciaDto">Dados para criação da assistência.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="201">Assistência criada com sucesso.</response>
        /// <response code="400">Dados inválidos ou duplicados.</response>
        /// <response code="500">Erro interno ao criar a assistência.</response>
        [HttpPost("CriarAssistencia")]
        public async Task<ActionResult> CriarAssistencia([FromBody] AssistenciaExternaDTO novaAssistenciaDto)
        {
            try
            {
                // Verifica se já existe assistência com o mesmo NIF
                var existente = await _context.AssistenciaExternas
                    .AnyAsync(a => a.Nif == novaAssistenciaDto.Nif);

                if (existente)
                    return BadRequest("Já existe uma assistência externa com o mesmo NIF.");

                var novaAssistencia = new AssistenciaExterna
                {
                    Nome = novaAssistenciaDto.Nome,
                    Nif = novaAssistenciaDto.Nif,
                    Morada = novaAssistenciaDto.Morada,
                    Telefone = novaAssistenciaDto.Telefone
                };

                _context.AssistenciaExternas.Add(novaAssistencia);
                await _context.SaveChangesAsync();

                return StatusCode(201, "Assistência externa criada com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao criar assistência externa: {ex.Message}");
            }
        }
        #endregion

        #region AtualizarAssistencia
        /// <summary>
        /// Atualiza os dados de uma assistência externa existente.
        /// </summary>
        /// <param name="assistenteId">ID da assistência a atualizar.</param>
        /// <param name="assistenciaAtualizada">Dados atualizados da assistência.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Assistência atualizada com sucesso.</response>
        /// <response code="404">Assistência não encontrada.</response>
        /// <response code="400">NIF duplicado ou dados inválidos.</response>
        /// <response code="500">Erro interno ao tentar atualizar a assistência.</response>
        [HttpPut("AtualizarAssistencia/{assistenteId}")]
        public async Task<IActionResult> AtualizarAssistencia(int assistenteId, [FromBody] AssistenciaExternaDTO assistenciaAtualizada)
        {
            try
            {
                var assistencia = await _context.AssistenciaExternas.FindAsync(assistenteId);

                if (assistencia == null)
                    return NotFound("Assistência externa não encontrada.");

                // Verificar duplicação de NIF (em outro registo)
                bool nifDuplicado = await _context.AssistenciaExternas
                    .AnyAsync(a => a.Nif == assistenciaAtualizada.Nif && a.AssistenteId != assistenteId);

                if (nifDuplicado)
                    return BadRequest("Já existe outra assistência externa com o mesmo NIF.");

                // Atualizar os campos
                assistencia.Nome = assistenciaAtualizada.Nome;
                assistencia.Nif = assistenciaAtualizada.Nif;
                assistencia.Morada = assistenciaAtualizada.Morada;
                assistencia.Telefone = assistenciaAtualizada.Telefone;

                await _context.SaveChangesAsync();

                return Ok("Assistência externa atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar assistência externa: {ex.Message}");
            }
        }
        #endregion
    }
}
