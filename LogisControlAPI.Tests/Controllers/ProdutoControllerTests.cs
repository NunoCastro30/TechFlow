using Xunit;
using LogisControlAPI.Controllers;
using LogisControlAPI.Data;
using LogisControlAPI.Models;
using LogisControlAPI.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Testes unitários para o ProdutoController.
/// Valida os métodos de listagem, criação, atualização, obtenção e remoção de produtos.
/// </summary>
public class ProdutoControllerTests
{
    private LogisControlContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LogisControlContext>()
            .UseInMemoryDatabase(databaseName: "ProdutoDb_" + System.Guid.NewGuid())
            .Options;

        return new LogisControlContext(options);
    }

    /// <summary>
    /// Verifica se o método GetAll retorna uma lista com os produtos existentes.
    /// </summary>
    [Fact]
    public async Task GetAll_DeveRetornarListaDeProdutos()
    {
        var context = GetInMemoryDbContext();

        context.Produtos.Add(new Produto
        {
            Nome = "Produto A",
            Quantidade = 10,
            CodInterno = "P001",
            Descricao = "Produto de teste",
            Preco = 20
        });
        await context.SaveChangesAsync();

        var controller = new ProdutoController(context);
        var resultado = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var lista = Assert.IsAssignableFrom<IEnumerable<ProdutoDTO>>(okResult.Value);
        Assert.Single(lista);
    }

    /// <summary>
    /// Verifica se o método GetById retorna o produto correto quando ele existe.
    /// </summary>
    [Fact]
    public async Task GetById_DeveRetornarProduto_SeExistir()
    {
        var context = GetInMemoryDbContext();
        var produto = new Produto
        {
            Nome = "Produto X",
            Quantidade = 5,
            CodInterno = "PX",
            Descricao = "Teste",
            Preco = 12
        };
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();

        var controller = new ProdutoController(context);
        var resultado = await controller.GetById(produto.ProdutoId);

        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var produtoDTO = Assert.IsType<Produto>(okResult.Value);
        Assert.Equal("Produto X", produtoDTO.Nome);
    }

    /// <summary>
    /// Verifica se o método GetById retorna NotFound quando o produto não existe.
    /// </summary>
    [Fact]
    public async Task GetById_DeveRetornarNotFound_SeNaoExistir()
    {
        var context = GetInMemoryDbContext();
        var controller = new ProdutoController(context);

        var resultado = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(resultado.Result);
    }

    /// <summary>
    /// Verifica se o método Create cria um novo produto e retorna o resultado esperado.
    /// </summary>
    [Fact]
    public async Task Create_DeveCriarNovoProdutoERetornarCreatedAt()
    {
        var context = GetInMemoryDbContext();
        var controller = new ProdutoController(context);

        var dto = new ProdutoDTO
        {
            Nome = "Novo Produto",
            Quantidade = 7,
            CodInterno = "NP123",
            Descricao = "Produto criado via teste",
            Preco = 9.99
        };

        var resultado = await controller.Create(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
        var produto = Assert.IsType<Produto>(createdResult.Value);
        Assert.Equal("Novo Produto", produto.Nome);
    }

    /// <summary>
    /// Verifica se o método Update atualiza um produto existente e retorna NoContent.
    /// </summary>
    [Fact]
    public async Task Update_DeveRetornarNoContent_SeAtualizar()
    {
        var context = GetInMemoryDbContext();
        var produto = new Produto
        {
            Nome = "Atualizar",
            Quantidade = 2,
            CodInterno = "AT001",
            Descricao = "Original",
            Preco = 5
        };
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();

        var controller = new ProdutoController(context);

        var dto = new ProdutoDTO
        {
            Nome = "Atualizado",
            Quantidade = 5,
            CodInterno = "XXX",
            Descricao = "Alterado",
            Preco = 15
        };

        var resultado = await controller.Update(produto.ProdutoId, dto);

        Assert.IsType<NoContentResult>(resultado);
        Assert.Equal("Atualizado", context.Produtos.Find(produto.ProdutoId)?.Nome);
    }

    /// <summary>
    /// Verifica se o método Update retorna NotFound ao tentar atualizar um produto inexistente.
    /// </summary>
    [Fact]
    public async Task Update_DeveRetornarNotFound_SeProdutoNaoExistir()
    {
        var context = GetInMemoryDbContext();
        var controller = new ProdutoController(context);

        var dto = new ProdutoDTO
        {
            Nome = "Inexistente",
            Quantidade = 0,
            CodInterno = "NA",
            Descricao = "Nada",
            Preco = 0
        };

        var resultado = await controller.Update(999, dto);

        Assert.IsType<NotFoundResult>(resultado);
    }

    /// <summary>
    /// Verifica se o método Delete remove um produto existente e retorna NoContent.
    /// </summary>
    [Fact]
    public async Task Delete_DeveRemoverProduto_SeExistir()
    {
        var context = GetInMemoryDbContext();
        var produto = new Produto
        {
            Nome = "Para Remover",
            Quantidade = 1,
            CodInterno = "RM001",
            Descricao = "Teste",
            Preco = 1
        };
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();

        var controller = new ProdutoController(context);

        var resultado = await controller.Delete(produto.ProdutoId);

        Assert.IsType<NoContentResult>(resultado);
        Assert.Null(context.Produtos.Find(produto.ProdutoId));
    }

    /// <summary>
    /// Verifica se o método Delete retorna NotFound ao tentar apagar um produto inexistente.
    /// </summary>
    [Fact]
    public async Task Delete_DeveRetornarNotFound_SeNaoExistir()
    {
        var context = GetInMemoryDbContext();
        var controller = new ProdutoController(context);

        var resultado = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(resultado);
    }
}
