using Microsoft.AspNetCore.Mvc;
using LogisControlAPI.Models;
using LogisControlAPI.Data;
using LogisControlAPI.DTO;
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
    public class EncomendaClienteController : ControllerBase
    {
        private readonly LogisControlContext _context;

        /// <summary>
        /// Construtor do controlador que injeta o contexto da base de dados.
        /// </summary>
        /// <param name="context">Instância do contexto da base de dados.</param>
        public EncomendaClienteController(LogisControlContext context)
        {
            _context = context;
        }

        [HttpGet("ListarEncomendas")]
        /// <summary>
        /// Lista todas as encomendas com o nome do cliente associado.
        /// </summary>
        /// <returns>Lista de encomendas.</returns>
        /// <response code="200">Lista obtida com sucesso.</response>
        /// <response code="500">Erro ao obter encomendas.</response>
        public async Task<ActionResult<IEnumerable<EncomendaClienteDTO>>> ListarEncomendas()
        {
            try
            {
                var encomendas = await _context.EncomendaClientes
                    .Include(e => e.ClienteCliente)
                    .Select(e => new EncomendaClienteDTO
                    {
                        EncomendaClienteId = e.EncomendaClienteId,
                        DataEncomenda = e.DataEncomenda,
                        Estado = e.Estado,
                        NomeCliente = e.ClienteCliente.Nome
                    })
                    .ToListAsync();

                return Ok(encomendas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter encomendas: {ex.Message}");
            }
        }



    }
}
