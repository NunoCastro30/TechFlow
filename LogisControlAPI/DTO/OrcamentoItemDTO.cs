namespace LogisControlAPI.DTO
{
    /// <summary>
    /// DTO para representar um item de orçamento.
    /// </summary>
    public class OrcamentoItemDTO
    {
        /// <summary>
        /// PK do item de orçamento.
        /// </summary>
        public int OrcamentoItemID { get; set; }

        /// <summary>
        /// Quantidade do item.
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Preço unitário do item.
        /// </summary>
        public double PrecoUnit { get; set; }

        /// <summary>
        /// Prazo de entrega (em dias). Pode ser nulo se não definido.
        /// </summary>
        public int? PrazoEntrega { get; set; }

        /// <summary>
        /// FK para o orçamento pai.
        /// </summary>
        public int OrcamentoID { get; set; }

        /// <summary>
        /// FK para a matéria-prima associada.
        /// </summary>
        public int MateriaPrimaID { get; set; }
    }
}