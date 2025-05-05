using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using LogisControlAPI.Data;
using LogisControlAPI.Services;
using LogisControlAPI.DTO;
using LogisControlAPI.Models;   

public class VerificacaoStockEncomendaServiceTests
{
    private LogisControlContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LogisControlContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
            .Options;

        return new LogisControlContext(options);
    }

    [Fact]
    public async Task VerificarStockParaEncomenda_DeveNotificar_SeStockInsuficiente()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        // Dados simulados: matéria-prima com stock insuficiente
        var materiaPrima = new MateriaPrima { MateriaPrimaId = 1, Nome = "Aço", Quantidade = 10,
            Descricao = "Aço carbono reforçado",
            Categoria = "Metais",
            CodInterno = "MP001",
            Preco = 12.5
        };
        var produto = new Produto
        {
            ProdutoId = 1,
            Nome = "Peça X",
            CodInterno = "P001",
            Descricao = "Descrição de teste",
            MateriaPrimaProdutos = new List<MateriaPrimaProduto>
        {
                new MateriaPrimaProduto
                {
                    MateriaPrimaMateriaPrimaId= 1,
                    QuantidadeNec = 20,
                    MateriaPrimaMateriaPrimaIDNavigation = materiaPrima
                }
            }
        };

        var item = new EncomendaItens
        {
            EncomendaClienteEncomendaClienteId = 1,
            Quantidade = 1,
            Produtos = new List<Produto> { produto }
        };

        var encomenda = new EncomendaCliente
        {
            EncomendaClienteId = 1,
            Estado = "Nova"
        };

        context.MateriasPrimas.Add(materiaPrima);
        context.Produtos.Add(produto);
        context.EncomendasItem.Add(item);
        context.EncomendasCliente.Add(encomenda);
        await context.SaveChangesAsync();

        var notificadorMock = new Mock<NotificationService>(null as object);
        var service = new VerificacaoStockEncomendaService(context, notificadorMock.Object);

        // Act
        await service.VerificarStockParaEncomenda(1);

        // Assert
        notificadorMock.Verify(n =>
            n.NotificarAsync(
                It.IsAny<string>(),
                It.Is<string>(s => s.Contains("Stock Insuficiente")),
                It.Is<string>(m => m.Contains("Aço"))
            ),
            Times.Once
        );

        var updated = await context.EncomendasCliente.FindAsync(1);
        Assert.Equal("Pendente por Falta de Stock", updated.Estado);
    }

    [Fact]
    public async Task VerificarStockParaEncomenda_NaoNotifica_SeStockSuficiente()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        var materiaPrima = new MateriaPrima { MateriaPrimaId = 1, Nome = "Cobre", Quantidade = 100,
            Descricao = "Aço carbono reforçado",
            Categoria = "Metais",
            CodInterno = "MP001",
            Preco = 12.5
        };
        var produto = new Produto
        {
            ProdutoId = 1,
            Nome = "Peça X",
            CodInterno = "P001",
            Descricao = "Descrição de teste",
            MateriaPrimaProdutos = new List<MateriaPrimaProduto>
        {
                new MateriaPrimaProduto
                {
                    MateriaPrimaMateriaPrimaId = 1,
                    QuantidadeNec = 10,
                    MateriaPrimaMateriaPrimaIDNavigation = materiaPrima
                }
            }
        };

        var item = new EncomendaItens
        {
            EncomendaClienteEncomendaClienteId = 1,
            Quantidade = 2,
            Produtos = new List<Produto> { produto }
        };

        var encomenda = new EncomendaCliente
        {
            EncomendaClienteId = 1,
            Estado = "Nova"
        };

        context.MateriasPrimas.Add(materiaPrima);
        context.Produtos.Add(produto);
        context.EncomendasItem.Add(item);
        context.EncomendasCliente.Add(encomenda);
        await context.SaveChangesAsync();

        var notificadorMock = new Mock<NotificationService>(null as object);
        var service = new VerificacaoStockEncomendaService(context, notificadorMock.Object);

        // Act
        await service.VerificarStockParaEncomenda(1);

        // Assert
        notificadorMock.Verify(n => n.NotificarAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        var updated = await context.EncomendasCliente.FindAsync(1);
        Assert.Equal("Nova", updated.Estado); // Estado permanece
    }
}
