using Microsoft.AspNetCore.Mvc;
using LogisControlAPI.Data;

namespace LogisControlAPI.Controllers 
{
    /// <summary>
    /// Controlador para testar a conectividade com a base de dados SQL Server.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TesteController : ControllerBase
    {
        private readonly LogisControlContext _context;

        /// <summary>
        /// Construtor que injeta o contexto da base de dados no controlador.
        /// </summary>
        /// <param name="context">Instância do AppDbContext para verificar a conexão com a BD.</param>
        public TesteController(LogisControlContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Endpoint para testar a conexão ao SQL Server.
        /// </summary>
        /// <returns>Mensagem indicando sucesso ou erro na conexão.</returns>
        /// <response code="200">Conexão bem-sucedida com a BD.</response>
        /// <response code="500">Erro ao tentar conectar a BD.</response>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            try
            {
                // Verifica se a API consegue estabelecer uma ligação à base de dados
                if (_context.Database.CanConnect())
                {
                    return Ok("✅ Ligação ao SQL Server estabelecida com sucesso!");
                }
                else
                {
                    return StatusCode(500, "❌ Falha na ligação ao SQL Server.");
                }
            }
            catch (Exception ex)
            {
                // Em caso de erro inesperado, retorna o código 500 e a mensagem do erro
                return StatusCode(500, $"❌ Erro: {ex.Message}");
            }
        }
    }
}