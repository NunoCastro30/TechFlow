using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LogisControlAPI.Models;
using LogisControlAPI.Data;

namespace LogisControlAPI.Services
{
    public class UtilizadorService
    {

        private readonly LogisControlContext _context;
        private readonly PasswordHasher<string> _passwordHasher;

        public UtilizadorService(LogisControlContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<string>();
        }

        //Verifica se um número de funcionário já existe
        public async Task<bool> VerificarSeExisteNumeroFuncionario(int numFuncionario)
        {
            return await _context.Utilizadores.AnyAsync(u => u.NumFuncionario == numFuncionario);
        }

        //Gera um hash seguro da pass
        public string HashPassword(string senha)
        {
            return _passwordHasher.HashPassword(null, senha);
        }

        public bool VerifyPassword(string hashedPassword, string inputPassword)
        {
            return _passwordHasher.VerifyHashedPassword(null, hashedPassword, inputPassword) == PasswordVerificationResult.Success;
        }

    }
}

