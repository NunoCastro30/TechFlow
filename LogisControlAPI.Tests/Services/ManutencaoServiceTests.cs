using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LogisControlAPI.Data;
using LogisControlAPI.Services;
using LogisControlAPI.Models;
using LogisControlAPI.DTO;
using LogisControlAPI.Interfaces; // Interface do TelegramService

public class ManutencaoServiceTests
{
    private LogisControlContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LogisControlContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new LogisControlContext(options);
    }

    [Fact]
    public async Task ObterPedidosAtrasadosAsync_DeveRetornarSomentePedidosNaoResolvidosComMaisDe7Dias()
    {
        var context = GetInMemoryDbContext();

        var dataAntiga = DateTime.UtcNow.AddDays(-10);
        var dataRecente = DateTime.UtcNow.AddDays(-3);

        context.PedidosManutencao.AddRange(
            new PedidoManutencao { PedidoManutId = 1, Estado = "Aberto", DataAbertura = dataAntiga, Descricao = "Teste A" },
            new PedidoManutencao { PedidoManutId = 2, Estado = "Concluido", DataAbertura = dataAntiga, Descricao = "Teste B" },
            new PedidoManutencao { PedidoManutId = 3, Estado = "Em Espera", DataAbertura = dataRecente, Descricao = "Teste C" }
        );
        await context.SaveChangesAsync();

        var telegramMock = new Mock<ITelegramService>();
        var service = new ManutencaoService(context, telegramMock.Object);

        var atrasados = await service.ObterPedidosAtrasadosAsync();

        Assert.Single(atrasados);
        Assert.Equal(1, atrasados[0].PedidoManutId);
    }

    [Fact]
    public async Task CriarPedidoAsync_DeveSalvarPedidoEEnviarMensagem()
    {
        var context = GetInMemoryDbContext();
        context.Maquinas.Add(new Maquina { MaquinaId = 1, Nome = "Torno CNC" });
        await context.SaveChangesAsync();

        var telegramMock = new Mock<ITelegramService>();
        telegramMock.Setup(t => t.EnviarMensagemAsync(It.IsAny<string>(), It.IsAny<string>()))
                     .Returns(Task.CompletedTask);

        var service = new ManutencaoService(context, telegramMock.Object);

        var dto = new PedidoManutençãoDTO { Descricao = "Falha no motor", MaquinaMaquinaId = 1 };
        await service.CriarPedidoAsync(dto, 42);

        var pedidoCriado = await context.PedidosManutencao.FirstOrDefaultAsync();
        Assert.NotNull(pedidoCriado);
        Assert.Equal("Falha no motor", pedidoCriado.Descricao);
        telegramMock.Verify(t => t.EnviarMensagemAsync(It.Is<string>(m => m.Contains("Falha no motor")), "Manutencao"), Times.Once);
    }

    [Fact]
    public async Task AtualizarEstadoPedidoSeRegistoResolvido_DeveAtualizarPedidoEEnviarMensagem()
    {
        var context = GetInMemoryDbContext();
        context.Maquinas.Add(new Maquina { MaquinaId = 1, Nome = "Impressora 3D" });

        var pedido = new PedidoManutencao
        {
            PedidoManutId = 10,
            Estado = "Em Espera",
            MaquinaMaquinaId = 1,
            Descricao = "teste"
        };

        context.PedidosManutencao.Add(pedido);
        context.RegistosManutencao.Add(new RegistoManutencao
        {
            RegistoManutencaoId = 100,
            Estado = "Resolvido",
            Descricao = "Troca de fusível",
            PedidoManutencaoPedidoManut = pedido
        });
        await context.SaveChangesAsync();

        var telegramMock = new Mock<ITelegramService>();
        telegramMock.Setup(t => t.EnviarMensagemAsync(It.IsAny<string>(), "Producao"))
            .Returns(Task.CompletedTask);

        var service = new ManutencaoService(context, telegramMock.Object);

        await service.AtualizarEstadoPedidoSeRegistoResolvido(100);

        var pedidoAtualizado = await context.PedidosManutencao.FindAsync(10);
        Assert.Equal("Concluido", pedidoAtualizado.Estado);
        Assert.NotNull(pedidoAtualizado.DataConclusao);
        telegramMock.Verify(t => t.EnviarMensagemAsync(It.Is<string>(m => m.Contains("Troca de fusível")), "Producao"), Times.Once);
    }
}
