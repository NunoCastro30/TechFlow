using System.ComponentModel.DataAnnotations;

namespace LogisControlAPI.DTO
{
    public class CriarUtilizadorDTO
    {
        [Required]
        public string PrimeiroNome { get; set; }

        [Required]
        public string Sobrenome { get; set; }

        [Required]
        public int NumFuncionario { get; set; }  // Número único do funcionário

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
