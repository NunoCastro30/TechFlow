using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LogisControlAPI.Data;
using LogisControlAPI.Models;
using LogisControlAPI.DTO;
using LogisControlAPI.Services;
using LogisControlAPI.Interfaces;
using System.Collections.Generic;
using System.Linq;

public class ProdutoServiceTests
{
    private LogisControlContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LogisControlContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
        return new LogisControlContext(options);
    }

    [Fact]
    public async Task CriarProdutoAsync_DeveCriarProdutoComMateriasPrimas()
    {
        var context = GetInMemoryDbContext();
        var stockMock = new Mock<IStockService>();
        var service = new ProdutoService(context, stockMock.Object);

        var dto = new CriarProdutoDTO
        {
            Nome = "Produto A",
            Quantidade = 10,
            Descricao = "Desc",
            CodInterno = "PA001",
            Preco = 50.5,
            MateriasPrimas = new List<MateriaPrimaProdutoCriacaoDTO>
            {
                new MateriaPrimaProdutoCriacaoDTO { MateriaPrimaId = 1, QuantidadeNec = 2 },
                new MateriaPrimaProdutoCriacaoDTO { MateriaPrimaId = 2, QuantidadeNec = 3 }
            }
        };

        context.MateriasPrimas.AddRange(
            new MateriaPrima { MateriaPrimaId = 1, Nome = "MP1", Quantidade = 100, Categoria = "Cat", CodInterno = "MP001", Descricao = "Desc" },
            new MateriaPrima { MateriaPrimaId = 2, Nome = "MP2", Quantidade = 100, Categoria = "Cat", CodInterno = "MP002", Descricao = "Desc" }
        );
        await context.SaveChangesAsync();

        await service.CriarProdutoAsync(dto);

        var produto = await context.Produtos.Include(p => p.MateriaPrimaProdutos).FirstOrDefaultAsync();
        Assert.NotNull(produto);
        Assert.Equal("Produto A", produto.Nome);
        Assert.Equal(2, produto.MateriaPrimaProdutos.Count);
    }

    [Fact]
    public async Task AtualizarProdutoAsync_DeveAtualizarDadosEVerificarStock()
    {
        var context = GetInMemoryDbContext();
        var stockMock = new Mock<IStockService>();
        stockMock.Setup(s => s.VerificarStockCriticoProduto(It.IsAny<int>(), It.IsAny<int>()))
                 .Returns(Task.CompletedTask);

        var produto = new Produto { ProdutoId = 1, Nome = "Old", Quantidade = 5, Descricao = "Old", CodInterno = "OLD001", Preco = 10 };
        context.Produtos.Add(produto);
        context.MateriaPrimaProdutos.Add(new MateriaPrimaProduto { ProdutoProdutoId = 1, MateriaPrimaMateriaPrimaId = 1, QuantidadeNec = 1 });
        await context.SaveChangesAsync();

        var service = new ProdutoService(context, stockMock.Object);

        var dto = new CriarProdutoDTO
        {
            Nome = "Novo",
            Quantidade = 20,
            Descricao = "Atualizado",
            CodInterno = "NEW001",
            Preco = 99.9,
            MateriasPrimas = new List<MateriaPrimaProdutoCriacaoDTO>
            {
                new MateriaPrimaProdutoCriacaoDTO { MateriaPrimaId = 2, QuantidadeNec = 4 }
            }
        };

        context.MateriasPrimas.Add(new MateriaPrima { MateriaPrimaId = 2, Nome = "MP2", Quantidade = 100, Categoria = "Cat", CodInterno = "MP002", Descricao = "Desc" });
        await context.SaveChangesAsync();

        await service.AtualizarProdutoAsync(1, dto);

        var produtoAtualizado = await context.Produtos.Include(p => p.MateriaPrimaProdutos).FirstAsync();
        Assert.Equal("Novo", produtoAtualizado.Nome);
        Assert.Equal(20, produtoAtualizado.Quantidade);
        Assert.Single(produtoAtualizado.MateriaPrimaProdutos);

        stockMock.Verify(s => s.VerificarStockCriticoProduto(1, 5), Times.Once);
    }
}
