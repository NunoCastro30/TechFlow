using Microsoft.AspNetCore.Mvc;
using LogisControlAPI.Models;
using LogisControlAPI.Data;
using LogisControlAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisControlAPI.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão dos clientes.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly LogisControlContext _context;

        /// <summary>
        /// Construtor do controlador que injeta o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da base de dados.</param>
        public ClienteController(LogisControlContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém a lista de todos os clientes registados.
        /// </summary>
        /// <returns>Lista de clientes sem encomendas associadas.</returns>
        /// <response code="200">Retorna a lista de clientes com sucesso.</response>
        /// <response code="500">Erro interno ao tentar obter os clientes.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> GetClientes()
        {
            try
            {
                var clientes = await _context.Clientes
                    .Select(c => new ClienteDTO
                    {
                        ClienteId = c.ClienteId,
                        Nome = c.Nome,
                        Nif = c.Nif,
                        Morada = c.Morada
                    })
                    .ToListAsync();

                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao obter clientes: {ex.Message}");
            }
        }
    }
}