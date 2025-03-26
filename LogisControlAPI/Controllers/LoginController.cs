using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Controllers
{
    public class LoginController : ControllerBase
    {

        private readonly LogisControlContext _context;
        private readonly UtilizadorService _utilizadorService;
        private readonly AuthService _authService;

        public LoginController(LogisControlContext context, UtilizadorService utilizadorService, AuthService authService)
        {
            _context = context;
            _utilizadorService = utilizadorService;
            _authService = authService;
        }

        /// <summary>
        /// Efetuar Login
        /// </summary>
        /// <returns>Efetua login na aplicação.</returns>
        /// <response code="200">Login efetuado com sucesso.</response>
        /// <response code="500">Erro ao efetuar login.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            // Procura o utilizador pelo número de funcionário
            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.NumFuncionario == loginDto.NumFuncionario);

            if (utilizador == null)
                return Unauthorized("Número de funcionário ou senha inválidos.");

            // Verifica se a senha está correta
            bool senhaCorreta = _utilizadorService.VerifyPassword(utilizador.Password, loginDto.Password);

            if (!senhaCorreta)
                return Unauthorized("Número de funcionário ou senha inválidos.");

            // Gera o token JWT usando o AuthService
            var token = _authService.GenerateToken(utilizador.NumFuncionario, utilizador.Role);

            // Se chegou aqui, o login está válido
            // Retorna o JSON com as informações necessárias
            // Retorna o token
            return Ok(new
            {
                Token = token
            });
        }
    }
}
