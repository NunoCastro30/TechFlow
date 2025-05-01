using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LogisControlAPI.Data;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using LogisControlAPI.Interfaces;

namespace LogisControlAPI.Services
{
    /// <summary>
    /// Serviço que encapsula toda a lógica de negócio para a gestão de compras.
    /// </summary>
    public class ComprasService
    {
        private readonly LogisControlContext _ctx;
        private readonly IEmailSender _emailSender;

        public ComprasService(LogisControlContext ctx, IEmailSender emailSender)
        {
            _ctx = ctx;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Lista todos os pedidos de compra filtrados por estado.
        /// </summary>
        public async Task<List<PedidoCompraDTO>> ListarPedidosPorEstadoAsync(string estado)
        {
            return await _ctx.PedidosCompra
                .AsNoTracking()
                .Include(p => p.UtilizadorUtilizador)
                .Where(p => p.Estado == estado)
                .Select(p => new PedidoCompraDTO
                {
                    PedidoCompraId = p.PedidoCompraId,
                    Descricao = p.Descricao,
                    Estado = p.Estado,
                    DataAbertura = p.DataAbertura,
                    DataConclusao = p.DataConclusao,
                    NomeUtilizador = $"{p.UtilizadorUtilizador.PrimeiroNome} {p.UtilizadorUtilizador.Sobrenome}"
                })
                .ToListAsync();
        }

        /// <summary>
        /// Obtém o detalhe completo de um pedido de compra, incluindo itens.
        /// </summary>
        public async Task<PedidoCompraDetalheDTO?> ObterPedidoCompraDetalheAsync(int id)
        {
            var pedido = await _ctx.PedidosCompra
                .AsNoTracking()
                .Include(p => p.UtilizadorUtilizador)
                .Include(p => p.PedidoCompraItems)
                    .ThenInclude(i => i.MateriaPrima)
                .FirstOrDefaultAsync(p => p.PedidoCompraId == id);

            if (pedido == null)
                return null;

            return new PedidoCompraDetalheDTO
            {
                PedidoCompraId = pedido.PedidoCompraId,
                Descricao = pedido.Descricao,
                Estado = pedido.Estado,
                DataAbertura = pedido.DataAbertura,
                DataConclusao = pedido.DataConclusao,
                NomeUtilizador = $"{pedido.UtilizadorUtilizador.PrimeiroNome} {pedido.UtilizadorUtilizador.Sobrenome}",
                Itens = pedido.PedidoCompraItems.Select(i => new ItemPedidoDetalheDTO
                {
                    MateriaPrimaId = i.MateriaPrimaId,
                    MateriaPrimaNome = i.MateriaPrima.Nome,
                    Quantidade = i.Quantidade
                }).ToList()
            };
        }

        /// <summary>
        /// Cria um novo pedido de compra (cabeçalho + itens).
        /// </summary>
        public async Task<int> CriarPedidoCompraAsync(CriarPedidoCompraDTO dto)
        {
            // 1) Cria e guarda o cabeçalho do pedido
            var pedido = new PedidoCompra
            {
                Descricao = dto.Descricao,
                DataAbertura = DateTime.UtcNow,
                Estado = "Aberto",
                UtilizadorUtilizadorId = dto.UtilizadorId
            };
            _ctx.PedidosCompra.Add(pedido);
            await _ctx.SaveChangesAsync();

            // 2) Cria e guarda cada item
            foreach (var item in dto.Itens)
            {
                _ctx.PedidoCompraItems.Add(new PedidoCompraItem
                {
                    PedidoCompraId = pedido.PedidoCompraId,
                    MateriaPrimaId = item.MateriaPrimaId,
                    Quantidade = item.Quantidade
                });
            }
            await _ctx.SaveChangesAsync();

            return pedido.PedidoCompraId;
        }

        /// <summary>
        /// Gera um pedido de cotação: muda o estado do pedido de compra,
        /// verifica existência do fornecedor, cria o cabeçalho em PedidoCotacao
        /// e retorna também um token de acesso.
        /// </summary>
        public async Task<(int CotacaoId, string Token)> CriarPedidoCotacaoAsync(int pedidoCompraId, int fornecedorId)
        {
            // 1) Verifica se o pedido de compra existe e está "Aberto"
            var pedido = await _ctx.PedidosCompra.FindAsync(pedidoCompraId);
            if (pedido == null)
                throw new KeyNotFoundException($"Pedido de compra {pedidoCompraId} não encontrado.");
            if (pedido.Estado != "Aberto")
                throw new InvalidOperationException("Pedido deve estar em estado 'Aberto' para gerar cotação.");

            // 2) Verifica se o fornecedor existe
            bool fornecedorExiste = await _ctx.Fornecedores
                .AnyAsync(f => f.FornecedorId == fornecedorId);
            if (!fornecedorExiste)
                throw new KeyNotFoundException($"Fornecedor {fornecedorId} não encontrado.");

            // 3) Atualiza o estado do pedido de compra e persiste imediatamente
            pedido.Estado = "EmCotacao";
            await _ctx.SaveChangesAsync();

            // 4) Gera token e cria o cabeçalho em PedidoCotacao
            var token = Guid.NewGuid().ToString("N");
            var cotacao = new PedidoCotacao
            {
                Descricao = pedido.Descricao,
                Data = DateTime.UtcNow,
                Estado = "Emitido",
                FornecedorId = fornecedorId,
                TokenAcesso = token
            };
            _ctx.PedidosCotacao.Add(cotacao);
            await _ctx.SaveChangesAsync();

            // 5) Enviar email ao fornecedor
            var fornecedor = await _ctx.Fornecedores.FindAsync(fornecedorId);
            if (fornecedor != null && !string.IsNullOrWhiteSpace(fornecedor.Email))
            {
                var link = $"http://localhost:5173/fornecedor/cotacao/{cotacao.PedidoCotacaoId}?token={token}";

                var mensagem = $"Caro fornecedor {fornecedor.Nome},\n\n" +
                               $"Foi-lhe atribuído um pedido de cotação. " +
                               $"Clique no link abaixo para aceder ao pedido:\n\n{link}\n\n" +
                               "Este link é exclusivo para si.";

                try
                {
                    Console.WriteLine($"A enviar e-mail para: {fornecedor.Email}");
                    await _emailSender.EnviarAsync(fornecedor.Email, "Novo Pedido de Cotação", mensagem);
                    Console.WriteLine("Email enviado com sucesso!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERRO AO ENVIAR EMAIL:");
                    Console.WriteLine(ex.ToString());
                }

            }


            return (cotacao.PedidoCotacaoId, token);
        }

        /// <summary>
        /// Obtém um pedido de cotação com orçamentos e seus itens.
        /// </summary>
        public async Task<PedidoCotacaoDetalhadoDTO> ObterPedidoCotacaoDetalhadoAsync(int cotacaoId)
        {
            var cot = await _ctx.PedidosCotacao
                .AsNoTracking()
                .Include(c => c.Orcamentos)
                    .ThenInclude(o => o.OrcamentoItems)
                .FirstOrDefaultAsync(c => c.PedidoCotacaoId == cotacaoId);

            if (cot == null)
                throw new KeyNotFoundException($"Pedido de cotação {cotacaoId} não encontrado.");

            return new PedidoCotacaoDetalhadoDTO
            {
                Header = new PedidoCotacaoDTO
                {
                    PedidoCotacaoID = cot.PedidoCotacaoId,
                    Descricao = cot.Descricao,
                    Data = cot.Data,
                    Estado = cot.Estado,
                    FornecedorID = cot.FornecedorId,
                    TokenAcesso = cot.TokenAcesso
                },
                Orcamentos = cot.Orcamentos.Select(o => new OrcamentoDTO
                {
                    OrcamentoID = o.OrcamentoID,
                    PedidoCotacaoID = o.PedidoCotacaoPedidoCotacaoID,
                    Data = o.Data,
                    Estado = o.Estado
                }).ToList(),
                Itens = cot.Orcamentos
                    .SelectMany(o => o.OrcamentoItems)
                    .Select(i => new OrcamentoItemDTO
                    {
                        OrcamentoItemID = i.OrcamentoItemID,
                        OrcamentoID = i.OrcamentoOrcamentoID,
                        MateriaPrimaID = i.MateriaPrimaID,
                        Quantidade = i.Quantidade,
                        PrecoUnit = i.PrecoUnit,
                        PrazoEntrega = i.PrazoEntrega ?? 0
                    })
                    .ToList()
            };
        }

        public async Task<PedidoCotacaoDetalhadoDTO> ObterPedidoCotacaoParaFornecedorAsync(int id, string token)
        {
            Console.WriteLine("[Fornecedor] A iniciar validação da cotação...");
            Console.WriteLine($"ID Recebido: {id}");
            Console.WriteLine($"Token Recebido: {token}");

            // 1) Buscar cotação com orçamentos
            var cot = await _ctx.PedidosCotacao
                .Include(c => c.Fornecedor)
                .Include(c => c.Orcamentos)
                    .ThenInclude(o => o.OrcamentoItems)
                .FirstOrDefaultAsync(c => c.PedidoCotacaoId == id);

            if (cot == null)
                throw new KeyNotFoundException();

            if (cot.TokenAcesso != token)
                throw new UnauthorizedAccessException();

            Console.WriteLine("Token válido. Cotação encontrada.");

            // 2) Recolher o pedido de compra com base na descrição
            var pedidoCompra = await _ctx.PedidosCompra
                .Include(p => p.PedidoCompraItems)
                    .ThenInclude(i => i.MateriaPrima)
                .Where(p => p.Descricao == cot.Descricao)
                .OrderByDescending(p => p.DataAbertura)
                .FirstOrDefaultAsync();

            if (pedidoCompra == null)
                throw new Exception("Pedido de compra correspondente não encontrado.");

            Console.WriteLine("Pedido de compra correspondente encontrado.");

            // 3) Construir DTO
            return new PedidoCotacaoDetalhadoDTO
            {
                Header = new PedidoCotacaoDTO
                {
                    PedidoCotacaoID = cot.PedidoCotacaoId,
                    Descricao = cot.Descricao,
                    Data = cot.Data,
                    Estado = cot.Estado,
                    FornecedorID = cot.FornecedorId,
                    TokenAcesso = cot.TokenAcesso
                },
                Orcamentos = cot.Orcamentos.Select(o => new OrcamentoDTO
                {
                    OrcamentoID = o.OrcamentoID,
                    PedidoCotacaoID = o.PedidoCotacaoPedidoCotacaoID,
                    Data = o.Data,
                    Estado = o.Estado
                }).ToList(),
                Itens = pedidoCompra.PedidoCompraItems.Select(i => new OrcamentoItemDTO
                {
                    MateriaPrimaID = i.MateriaPrimaId,
                    MateriaPrimaNome = i.MateriaPrima.Nome,
                    Quantidade = i.Quantidade,
                    PrecoUnit = 0,
                    PrazoEntrega = 0
                }).ToList()
            };
        }

        public async Task<int> AceitarOrcamentoAsync(int orcamentoId)
        {
            // 1) Carrega orçamento + itens
            var orc = await _ctx.Orcamentos
                .Include(o => o.OrcamentoItems)
                .FirstOrDefaultAsync(o => o.OrcamentoID == orcamentoId);
            if (orc == null) throw new KeyNotFoundException();

            // 2) Recusa todos os irmãos
            var orcamentosDoPedido = _ctx.Orcamentos
            .Where(o => o.PedidoCotacaoPedidoCotacaoID == orc.PedidoCotacaoPedidoCotacaoID);

            foreach (var orçamento in orcamentosDoPedido)
                orçamento.Estado = (orçamento.OrcamentoID == orcamentoId ? "Aceite" : "Recusado");

            // 3) Cria NotaEncomenda
            var nota = new NotaEncomenda
            {
                DataEmissao = DateTime.UtcNow,
                Estado = "Pendente",
                OrcamentoId = orcamentoId,
                ValorTotal = orc.OrcamentoItems.Sum(i => i.Quantidade * i.PrecoUnit)
            };
            _ctx.NotasEncomenda.Add(nota);
            await _ctx.SaveChangesAsync();

            // 4) Cria itens de nota
            foreach (var i in orc.OrcamentoItems)
            {
                _ctx.NotasEncomendaItem.Add(new NotaEncomendaItens
                {
                    NotaEncomendaId = nota.NotaEncomendaId,
                    MateriaPrimaId = i.MateriaPrimaID,
                    Quantidade = i.Quantidade,
                    PrecoUnit = i.PrecoUnit
                });
            }
            await _ctx.SaveChangesAsync();

            return nota.NotaEncomendaId;
        }

        public async Task<NotaEncomendaDetalheDTO> ObterNotaEncomendaAsync(int id)
        {
            var nota = await _ctx.NotasEncomenda
                .Include(n => n.Itens).ThenInclude(it => it.MateriaPrima)
                .FirstOrDefaultAsync(n => n.NotaEncomendaId == id);
            if (nota == null) throw new KeyNotFoundException();

            return new NotaEncomendaDetalheDTO
            {
                NotaEncomendaId = nota.NotaEncomendaId,
                DataEmissao = nota.DataEmissao,
                Estado = nota.Estado,
                ValorTotal = nota.ValorTotal,
                OrcamentoId = nota.OrcamentoId,
                Itens = nota.Itens.Select(it => new NotaEncomendaItemDTO
                {
                    MateriaPrimaId = it.MateriaPrimaId,
                    MateriaPrimaNome = it.MateriaPrima.Nome,
                    Quantidade = it.Quantidade,
                    PrecoUnit = it.PrecoUnit
                }).ToList()
            };
        }
    }
}