using Microsoft.AspNetCore.Mvc;
using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using Microsoft.EntityFrameworkCore;
using LogisControlAPI.Models;
using LogisControlAPI.Services;


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
        /// <summary>
        /// Obtém a lista de todos os utilizadores registados.
        /// </summary>
        /// <returns>Lista de utilizadores com dados públicos (sem password).</returns>
        /// <response code="200">Lista de utilizadores obtida com sucesso.</response>
        [HttpGet ("ObterUtilizadores")]
        [Produces("application/json")]
        public async Task<IEnumerable<UtilizadorDTO>> GetUtilizadores()
        {
            return await _context.Utilizadores
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
        /// <summary>
        /// Cria um novo utilizador com os dados fornecidos.
        /// </summary>
        /// <param name="novoUtilizadorDto">Dados do novo utilizador.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Utilizador criado com sucesso.</response>
        /// <response code="400">Número de funcionário já existe.</response>
        /// <response code="500">Erro interno ao criar o utilizador.</response>
        [HttpPost("CriarUtilizador")]
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

            _context.Utilizadores.Add(novoUtilizador);
            await _context.SaveChangesAsync();

            return Ok("Utilizador criado com sucesso!");
        }

        #endregion

        #region AtualizarPerfil
        /// <summary>
        /// Atualiza o perfil do utilizador: primeiro nome, sobrenome e/ou password.
        /// Apenas os campos preenchidos serão atualizados. A password é guardada com hash.
        /// </summary>
        /// <param name="id">ID do utilizador a atualizar.</param>
        /// <param name="dto">Dados a atualizar.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Perfil atualizado com sucesso.</response>
        /// <response code="404">Utilizador não encontrado.</response>
        /// <response code="500">Erro interno ao atualizar o perfil.</response>
        [HttpPut("AtualizarPerfil/{id}")]
        public async Task<IActionResult> AtualizarPerfil(int id, [FromBody] AtualizarPerfilUtilizadorDTO dto)
        {
            try
            {
                var utilizador = await _context.Utilizadores.FindAsync(id);
                if (utilizador == null)
                    return NotFound("Utilizador não encontrado.");

                // Atualiza os dados se forem fornecidos
                if (!string.IsNullOrWhiteSpace(dto.PrimeiroNome))
                    utilizador.PrimeiroNome = dto.PrimeiroNome;

                if (!string.IsNullOrWhiteSpace(dto.Sobrenome))
                    utilizador.Sobrenome = dto.Sobrenome;

                if (!string.IsNullOrWhiteSpace(dto.NovaPassword))
                {
                    // Usar o serviço para gerar o hash da nova password
                    var novaHash = _utilizadorService.HashPassword(dto.NovaPassword);
                    utilizador.Password = novaHash;
                }

                await _context.SaveChangesAsync();
                return Ok("Perfil atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar perfil: {ex.Message}");
            }
        }
        #endregion

        #region AtualizarEstadoRole
        /// <summary>
        /// Atualiza o estado e o perfil (role) de um utilizador. Apenas usado por administradores.
        /// </summary>
        /// <param name="id">ID do utilizador a atualizar.</param>
        /// <param name="dto">Novo role e estado.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Utilizador atualizado com sucesso.</response>
        /// <response code="404">Utilizador não encontrado.</response>
        /// <response code="500">Erro interno ao atualizar o utilizador.</response>
        [HttpPut("AtualizarEstadoRole/{id}")]
        public async Task<IActionResult> AtualizarEstadoRole(int id, [FromBody] UtilizadorUpdateAdminDTO dto)
        {
            try
            {
                var utilizador = await _context.Utilizadores.FindAsync(id);
                if (utilizador == null)
                    return NotFound("Utilizador não encontrado.");

                utilizador.Role = dto.Role;
                utilizador.Estado = dto.Estado;

                await _context.SaveChangesAsync();
                return Ok("Utilizador atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar o utilizador: {ex.Message}");
            }
        }
        #endregion

        #region LoginTestes

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