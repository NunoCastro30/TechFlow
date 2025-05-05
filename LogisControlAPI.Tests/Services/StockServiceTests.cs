﻿using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using LogisControlAPI.Data;
using LogisControlAPI.Services;
using LogisControlAPI.Models;
using System.Threading.Tasks;

/// <summary>
/// Conjunto de testes unitários para o serviço StockService.
/// Valida a emissão de alertas de stock crítico de matérias-primas.
/// </summary>
public class StockServiceTests
{
    /// <summary>
    /// Cria um contexto de base de dados em memória (InMemory) para testes isolados.
    /// </summary>
    private LogisControlContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LogisControlContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
            .Options;
        return new LogisControlContext(options);
    }

    /// <summary>
    /// Garante que, se a matéria-prima não existir, nenhuma notificação é enviada.
    /// </summary>
    [Fact]
    public async Task VerificarStockCritico_NaoFazNada_SeMateriaPrimaNaoExistir()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var notificadorMock = new Mock<NotificationService>(null as object);
        var service = new StockService(context, notificadorMock.Object);

        // Act
        await service.VerificarStockCritico(999, 20); // ID inexistente, valor anterior arbitrário

        // Assert
        notificadorMock.Verify(
            n => n.NotificarAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never()
        );
    }

    /// <summary>
    /// Garante que uma notificação é enviada se a quantidade da matéria-prima for inferior a 10 e diminuiu.
    /// </summary>
    [Fact]
    public async Task VerificarStockCritico_DeveEnviarNotificacao_SeQuantidadeInferiorA10()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.MateriasPrimas.Add(new MateriaPrima
        {
            MateriaPrimaId = 1,
            Nome = "Aço",
            Quantidade = 5,
            Categoria = "Metais",
            CodInterno = "AC001",
            Descricao = "Aço carbono para estruturas"
        });
        await context.SaveChangesAsync();

        var notificadorMock = new Mock<NotificationService>(null as object);
        var service = new StockService(context, notificadorMock.Object);

        // Act
        await service.VerificarStockCritico(1, 15); // Redução de 15 para 5

        // Assert
        notificadorMock.Verify(
            n => n.NotificarAsync(
                "nunofernandescastro@gmail.com",
                It.Is<string>(s => s.Contains("Stock Baixo")),
                It.Is<string>(m => m.Contains("Aço") && m.Contains("5"))
            ),
            Times.Once()
        );
    }

    /// <summary>
    /// Garante que nenhuma notificação é enviada se a quantidade for >= 10.
    /// </summary>
    [Fact]
    public async Task VerificarStockCritico_NaoEnviaNotificacao_SeQuantidadeIgualOuSuperiorA10()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.MateriasPrimas.Add(new MateriaPrima
        {
            MateriaPrimaId = 2,
            Nome = "Ferro",
            Quantidade = 12,
            Categoria = "Metais",
            CodInterno = "FE001",
            Descricao = "Ferro fundido para peças"
        });
        await context.SaveChangesAsync();

        var notificadorMock = new Mock<NotificationService>(null as object);
        var service = new StockService(context, notificadorMock.Object);

        // Act
        await service.VerificarStockCritico(2, 15); // Redução de 15 para 12, mas ainda acima do limiar

        // Assert
        notificadorMock.Verify(
            n => n.NotificarAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never()
        );
    }

    /// <summary>
    /// Garante que nenhuma notificação é enviada se a quantidade atual for menor que 10, mas não diminuiu.
    /// </summary>
    [Fact]
    public async Task VerificarStockCritico_NaoEnviaNotificacao_SeNaoHouveReducao()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.MateriasPrimas.Add(new MateriaPrima
        {
            MateriaPrimaId = 3,
            Nome = "Cobre",
            Quantidade = 8,
            Categoria = "Metais",
            CodInterno = "CB001",
            Descricao = "Cobre eletrolítico"
        });
        await context.SaveChangesAsync();

        var notificadorMock = new Mock<NotificationService>(null as object);
        var service = new StockService(context, notificadorMock.Object);

        // Act
        await service.VerificarStockCritico(3, 5); // Aumentou de 5 para 8

        // Assert
        notificadorMock.Verify(
            n => n.NotificarAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never()
        );
    }
}