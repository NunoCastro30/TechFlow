using LogisControlAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Services
{
    public class VerificacaoStockEncomendaService
    {
        private readonly LogisControlContext _context;
        private readonly NotificationService _notificador;

        /// <summary>
        /// Email fixo (temporário) do responsável de stock.
        /// </summary>
        private const string EmailResponsavelStock = "nunofernandescastro@gmail.com";

        public VerificacaoStockEncomendaService(LogisControlContext context, NotificationService notificador)
        {
            _context = context;
            _notificador = notificador;
        }

        public async Task VerificarStockParaEncomenda(int encomendaClienteId)
        {
            var itens = await _context.EncomendasItem
                .Where(ei => ei.EncomendaClienteEncomendaClienteId == encomendaClienteId)
                .Include(ei => ei.Produtos)
                    .ThenInclude(p => p.MateriaPrimaProdutos)
                        .ThenInclude(mp => mp.MateriaPrimaMateriaPrimaIDNavigation)
                .ToListAsync();

            var mensagens = new List<string>();

            foreach (var item in itens)
            {
                foreach (var produto in item.Produtos)
                {
                    foreach (var mp in produto.MateriaPrimaProdutos)
                    {
                        int quantidadeEncomenda = item.Quantidade ?? 0;

                        int stockDisponivel = mp.MateriaPrimaMateriaPrimaIDNavigation.Quantidade;
                        int stockNecessario = mp.QuantidadeNec * quantidadeEncomenda;

                        if (stockDisponivel < stockNecessario)
                        {
                            mensagens.Add(
                                $"Matéria-prima '{mp.MateriaPrimaMateriaPrimaIDNavigation.Nome}' necessita de {stockNecessario}, mas só existem {stockDisponivel} em stock para o produto '{produto.Nome}'."
                            );
                        }
                    }
                }
            }

            if (mensagens.Any())
            {
                string assunto = $"⚠️ Stock Insuficiente para Encomenda {encomendaClienteId}";
                string corpo = string.Join("\n", mensagens);
                await _notificador.NotificarAsync(EmailResponsavelStock, assunto, corpo);

                // Atualizar o estado da encomenda
                var encomenda = await _context.EncomendasCliente.FindAsync(encomendaClienteId);
                if (encomenda != null)
                {
                    encomenda.Estado = "Pendente por Falta de Stock";
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
