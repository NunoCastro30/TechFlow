using Microsoft.AspNetCore.Mvc;
using LogisControlAPI.Data;
using LogisControlAPI.DTOs;
using Microsoft.EntityFrameworkCore;


namespace LogisControlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilizadorController : ControllerBase
    {
        private readonly LogisControlContext _context;

        public UtilizadorController(LogisControlContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<IEnumerable<UtilizadorDTO>> GetUtilizadores()
        {
            return await _context.Utilizadors
                .Select(u => new UtilizadorDTO
                {
                    UtilizadorId = u.UtilizadorId,
                    PrimeiroNome = u.PrimeiroNome,
                    Sobrenome = u.Sobrenome,
                    NumFuncionario = u.NumFuncionario,
                    Role = u.Role,
                    Estado = u.Estado
                })
                .ToListAsync();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> AtualizarNomeSobrenome(int id, [FromBody] UtilizadorUpdateDTO updateDTO)
        {
            // Verifica se o ID do utilizador existe na base de dados
            var utilizador = await _context.Utilizadors.FindAsync(id);
            if (utilizador == null)
            {
                return NotFound("Utilizador não encontrado.");
            }

            // Atualiza os campos permitidos
            utilizador.PrimeiroNome = updateDTO.PrimeiroNome;
            utilizador.Sobrenome = updateDTO.Sobrenome;

            // Guarda as alterações na base de dados
            await _context.SaveChangesAsync();

            return Ok("Nome e sobrenome atualizados com sucesso.");
        }

    }
}
