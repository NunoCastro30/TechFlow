using LogisControlAPI.Data;
using System;
using System.Threading.Tasks;

namespace LogisControlAPI.Services
{
    /// <summary>
    /// Serviço responsável por verificar o stock de matérias-primas e emitir alertas se necessário.
    /// </summary>
    public class StockService
    {
        private readonly LogisControlContext _context;
        private readonly NotificationService _notificador;

        /// <summary>
        /// Email fixo (temporário) do responsável de stock.
        /// </summary>
        private const string EmailResponsavelStock = "nunofernandescastro@gmail.com";

        /// <summary>
        /// Construtor que injeta as dependências necessárias.
        /// </summary>
        /// <param name="context">Contexto da base de dados.</param>
        /// <param name="notificador">Serviço de notificações.</param>
        public StockService(LogisControlContext context, NotificationService notificador)
        {
            _context = context;
            _notificador = notificador;
        }

        /// <summary>
        /// Verifica se a quantidade de uma matéria-prima está abaixo do limite crítico (5 unidades) e envia um alerta se necessário.
        /// </summary>
        /// <param name="materiaPrimaId">ID da matéria-prima a verificar.</param>
        /// <returns>Uma tarefa assíncrona.</returns>
        public async Task VerificarStockCritico(int materiaPrimaId)
        {
            var materia = await _context.MateriasPrimas.FindAsync(materiaPrimaId);
            if (materia == null)
                return;

            if (materia.Quantidade < 10)
            {
                var assunto = $"Stock Baixo - {materia.Nome}";
                var mensagem = $"A matéria-prima \"{materia.Nome}\" tem apenas {materia.Quantidade} unidades em stock.";

                await _notificador.NotificarAsync(EmailResponsavelStock, assunto, mensagem);
            }
        }
    }
}
