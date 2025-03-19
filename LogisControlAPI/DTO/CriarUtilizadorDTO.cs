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
        [StringLength(8, MinimumLength = 3, ErrorMessage = "A Password deve ter exatamente 3 caracteres.")]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
