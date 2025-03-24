using Microsoft.AspNetCore.Mvc;
using LogisControlAPI.Data;
using LogisControlAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using LogisControlAPI.Models;
using LogisControlAPI.Services;
using LogisControlAPI.DTO;

namespace LogisControlAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilizadorController : ControllerBase
    {
        private readonly LogisControlContext _context;
        private readonly UtilizadorService _utilizadorService;

        public UtilizadorController(LogisControlContext context, UtilizadorService utilizadorService)
        {
            _context = context;
            _utilizadorService = utilizadorService;
        }

        #region Obter Utilizadores

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

        #endregion

        #region Criar Utilizador

        [HttpPost("criar-utilizador")]
        public async Task<IActionResult> CriarUtilizador([FromBody] CriarUtilizadorDTO novoUtilizadorDto)
        {
            // Verifica se o número de funcionário já existe
            if (await _utilizadorService.VerificarSeExisteNumeroFuncionario(novoUtilizadorDto.NumFuncionario))
                return BadRequest("Já existe um utilizador com esse número de funcionário.");

            // Gerar o hash da senha antes de guardar
            string senhaHash = _utilizadorService.HashPassword(novoUtilizadorDto.Password);

            // Criar novo utilizador com a senha hashada
            Utilizador novoUtilizador = new Utilizador
            {
                PrimeiroNome = novoUtilizadorDto.PrimeiroNome,
                Sobrenome = novoUtilizadorDto.Sobrenome,
                NumFuncionario = novoUtilizadorDto.NumFuncionario,
                Password = senhaHash,
                Role = novoUtilizadorDto.Role,
                Estado = true 
            };

            _context.Utilizadors.Add(novoUtilizador);
            await _context.SaveChangesAsync();

            return Ok("Utilizador criado com sucesso!");
        }

        #endregion

        #region Login

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            // Procura o utilizador pelo número de funcionário
            var utilizador = await _context.Utilizadors
                .FirstOrDefaultAsync(u => u.NumFuncionario == loginDto.NumFuncionario);

            if (utilizador == null)
                return Unauthorized("Número de funcionário ou senha inválidos.");

            // Verifica se a senha está correta
            bool senhaCorreta = _utilizadorService.VerifyPassword(utilizador.Password, loginDto.Password);

            if (!senhaCorreta)
                return Unauthorized("Número de funcionário ou senha inválidos.");

            // Se chegou aqui, o login está válido
            // Retorna o JSON com as informações necessárias
            var respostaLogin = new
            {
                Sucesso = true,
                Mensagem = "Login bem-sucedido!",
                UtilizadorId = utilizador.UtilizadorId,
                Nome = utilizador.PrimeiroNome + " " + utilizador.Sobrenome,
                Role = utilizador.Role,
                NumFuncionario = utilizador.NumFuncionario
            };

            return Ok(respostaLogin);
        }

        #endregion
    }
}