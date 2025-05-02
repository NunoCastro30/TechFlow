using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using LogisControlAPI.Data;
using LogisControlAPI.Services;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;
using LogisControlAPI.Interfaces;

public class ComprasServiceTests
{
    private LogisControlContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LogisControlContext>()
            .UseInMemoryDatabase("TestDb_" + System.Guid.NewGuid())
            .Options;
        return new LogisControlContext(options);
    }

    [Fact]
    public async Task ListarPedidosPorEstadoAsync_DeveRetornarSomentePedidosDoEstado()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        context.Utilizadores.Add(new Utilizador
        {
            UtilizadorId = 1,
            PrimeiroNome = "João",
            Sobrenome = "Silva",
            Password = "hashedpassword",
            Role = "Gestor",
        });

        context.PedidosCompra.AddRange(
            new PedidoCompra
            {
                PedidoCompraId = 1,
                Estado = "Aberto",
                Descricao = "Pedido A",
                UtilizadorUtilizadorId = 1,
                DataAbertura = DateTime.UtcNow
            },
            new PedidoCompra
            {
                PedidoCompraId = 2,
                Estado = "Concluido",
                Descricao = "Pedido B",
                UtilizadorUtilizadorId = 1,
                DataAbertura = DateTime.UtcNow
            }
        );

        await context.SaveChangesAsync();

        var emailSenderMock = new Mock<IEmailSender>();
        var service = new ComprasService(context, emailSenderMock.Object);

        // Act
        var resultado = await service.ListarPedidosPorEstadoAsync("Aberto");

        // Assert
        Assert.Single(resultado);
        Assert.Equal("Pedido A", resultado.First().Descricao);
        Assert.Equal("João Silva", resultado.First().NomeUtilizador);
    }

    [Fact]
    public async Task ObterPedidoCompraDetalheAsync_DeveRetornarNull_SeNaoEncontrar()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var emailSenderMock = new Mock<IEmailSender>();
        var service = new ComprasService(context, emailSenderMock.Object);

        // Act
        var resultado = await service.ObterPedidoCompraDetalheAsync(999); // ID inexistente

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObterPedidoCompraDetalheAsync_DeveRetornarDetalhes_SeEncontrar()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        var materiaPrima = new MateriaPrima
        {
            MateriaPrimaId = 1,
            Nome = "Aço",
            Categoria = "Metais",
            CodInterno = "AC001",
            Descricao = "Aço carbono de alta resistência"
        };
        var utilizador = new Utilizador { UtilizadorId = 1, PrimeiroNome = "Maria", Sobrenome = "Silva", Password ="hashedpassword", Role = "Gestor" };

        context.MateriasPrimas.Add(materiaPrima);
        context.Utilizadores.Add(utilizador);

        var pedido = new PedidoCompra
        {
            PedidoCompraId = 1,
            Descricao = "Compra A",
            Estado = "Aberto",
            DataAbertura = DateTime.UtcNow,
            UtilizadorUtilizadorId = 1,
            PedidoCompraItems = new List<PedidoCompraItem>
        {
            new PedidoCompraItem
            {
                MateriaPrimaId = 1,
                Quantidade = 5,
                MateriaPrima = materiaPrima
            }
        }
        };

        context.PedidosCompra.Add(pedido);
        await context.SaveChangesAsync();

        var emailSenderMock = new Mock<IEmailSender>();
        var service = new ComprasService(context, emailSenderMock.Object);

        // Act
        var resultado = await service.ObterPedidoCompraDetalheAsync(1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Compra A", resultado.Descricao);
        Assert.Single(resultado.Itens);
        Assert.Equal("Aço", resultado.Itens.First().MateriaPrimaNome);
    }

    [Fact]
    public async Task CriarPedidoCompraAsync_DeveCriarPedidoComItens()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Utilizadores.Add(new Utilizador { UtilizadorId = 1, PrimeiroNome = "Carlos", Sobrenome = "Souza", Password = "hashedpassword", Role = "Gestor" });
        context.MateriasPrimas.Add(new MateriaPrima { MateriaPrimaId = 1, Nome = "Aço", Categoria = "Metais", CodInterno = "M001", Descricao = "Metal Coreano"});
        await context.SaveChangesAsync();

        var emailSenderMock = new Mock<IEmailSender>();
        var service = new ComprasService(context, emailSenderMock.Object);

        var dto = new CriarPedidoCompraDTO
        {
            UtilizadorId = 1,
            Descricao = "Nova compra",
            Itens = new List<ItemPedidoDTO>
        {
            new ItemPedidoDTO { MateriaPrimaId = 1, Quantidade = 3 }
        }
        };

        // Act
        var id = await service.CriarPedidoCompraAsync(dto);

        // Assert
        var pedidoCriado = await context.PedidosCompra.Include(p => p.PedidoCompraItems).FirstOrDefaultAsync(p => p.PedidoCompraId == id);
        Assert.NotNull(pedidoCriado);
        Assert.Single(pedidoCriado.PedidoCompraItems);
        Assert.Equal("Nova compra", pedidoCriado.Descricao);
    }




}