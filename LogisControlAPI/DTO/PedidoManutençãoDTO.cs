namespace LogisControlAPI.DTO
{
    public class PedidoManutençãoDTO
    {
        public int PedidoManutId { get; set; }

        public string Descicao { get; set; } = null!;

        public string Estado { get; set; } = null!;

        public DateTime DataAbertura { get; set; }

        public DateTime? DataConclusao { get; set; }

        public int MaquinaMaquinaId { get; set; }

        public int UtilizadorUtilizadorId { get; set; }
    }
}
