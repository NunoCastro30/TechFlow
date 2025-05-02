using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LogisControlAPI.Data;
using LogisControlAPI.Models;
using LogisControlAPI.Services;

public class ProducaoServiceTests
{
    private LogisControlContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LogisControlContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new LogisControlContext(options);
    }

    [Fact]
    public async Task AtualizarEstadoEObservacoesAsync_DeveConcluirOrdemEAtualizarProduto_SeEstadoProduzido()
    {
        var context = GetInMemoryDbContext();

        var ordem = new OrdemProducao { OrdemProdId = 1, Quantidade = 10, Estado = "Em Producao" };
        var produto = new Produto { ProdutoId = 1, Quantidade = 5, OrdemProducaoOrdemProdId = 1, CodInterno = "1", Descricao = "teste", Nome = "teste" };
        var registo = new RegistoProducao { RegistoProducaoId = 1, Estado = "", OrdemProducaoOrdemProdId = 1 };

        context.OrdensProducao.Add(ordem);
        context.Produtos.Add(produto);
        context.RegistosProducao.Add(registo);
        await context.SaveChangesAsync();

        var notificadorMock = new Mock<NotificationService>(null as object);
        var service = new ProducaoService(context, notificadorMock.Object);

        await service.AtualizarEstadoEObservacoesAsync(1, "Produzido", "Finalizado com sucesso");

        var ordemAtualizada = await context.OrdensProducao.FindAsync(1);
        var produtoAtualizado = await context.Produtos.FindAsync(1);

        Assert.Equal("Concluido", ordemAtualizada.Estado);
        Assert.Equal(15, produtoAtualizado.Quantidade);
        Assert.NotNull(ordemAtualizada.DataConclusao);

        notificadorMock.Verify(n => n.NotificarAsync(
            It.Is<string>(email => email.Contains("nunofernandescastro")),
            It.Is<string>(a => a.Contains("Produção Concluída")),
            It.Is<string>(m => m.Contains("#1"))
        ), Times.Once);
    }

    [Fact]
    public async Task AtualizarEstadoEObservacoesAsync_DeveCancelarOrdemENotificar_SeEstadoCancelado()
    {
        var context = GetInMemoryDbContext();

        var ordem = new OrdemProducao { OrdemProdId = 2, Quantidade = 5, Estado = "Em Producao" };
        var registo = new RegistoProducao { RegistoProducaoId = 2, Estado = "", OrdemProducaoOrdemProdId = 2 };

        context.OrdensProducao.Add(ordem);
        context.RegistosProducao.Add(registo);
        await context.SaveChangesAsync();

        var notificadorMock = new Mock<NotificationService>(null as object);
        var service = new ProducaoService(context, notificadorMock.Object);

        await service.AtualizarEstadoEObservacoesAsync(2, "Cancelado", "Problema de qualidade");

        var ordemAtualizada = await context.OrdensProducao.FindAsync(2);
        Assert.Equal("Cancelada", ordemAtualizada.Estado);

        notificadorMock.Verify(n => n.NotificarAsync(
            It.Is<string>(email => email.Contains("nunofernandescastro")),
            It.Is<string>(a => a.Contains("Produção Cancelada")),
            It.Is<string>(m => m.Contains("#2"))
        ), Times.Once);
    }

    [Fact]
    public async Task AtualizarEstadoEObservacoesAsync_DeveNotificar_SeEstadoParadoPorDefeito()
    {
        var context = GetInMemoryDbContext();

        var ordem = new OrdemProducao { OrdemProdId = 3, Quantidade = 8, Estado = "Em Producao" };
        var registo = new RegistoProducao { RegistoProducaoId = 3, Estado = "", OrdemProducaoOrdemProdId = 3 };

        context.OrdensProducao.Add(ordem);
        context.RegistosProducao.Add(registo);
        await context.SaveChangesAsync();

        var notificadorMock = new Mock<NotificationService>(null as object);
        var service = new ProducaoService(context, notificadorMock.Object);

        await service.AtualizarEstadoEObservacoesAsync(3, "Parado devido defeito", "Falha mecânica");

        notificadorMock.Verify(n => n.NotificarAsync(
            It.Is<string>(email => email.Contains("nunofernandescastro")),
            It.Is<string>(a => a.Contains("Produção Parada devido a Defeito")),
            It.Is<string>(m => m.Contains("#3"))
        ), Times.Once);
    }
}

