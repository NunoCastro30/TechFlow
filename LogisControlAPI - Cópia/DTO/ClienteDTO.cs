namespace LogisControlAPI.DTOs
{
    /// <summary>
    /// DTO para representar um utilizador sem informações sensíveis.
    /// </summary>

    public class ClienteDTO
    {
        public int ClienteId { get; set; }
        public string Nome { get; set; } = null!;
        public int Nif { get; set; }
        public string Morada { get; set; } = null!;
    }
}