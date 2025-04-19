using Xunit;
using LogisControlAPI.Controllers;
using LogisControlAPI.Data;
using LogisControlAPI.Models;
using LogisControlAPI.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Testes unitários para o PedidoCompraController.
/// Valida a criação, listagem e atualização de pedidos de compra.
/// </summary>
public class PedidoCompraControllerTests
{
    /// <summary>
    /// Cria um contexto EF Core InMemory isolado para cada teste.
    /// </summary>
    private LogisControlContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LogisControlContext>()
            .UseInMemoryDatabase(databaseName: "PedidoCompraDb_" + System.Guid.NewGuid())
            .Options;

        return new LogisControlContext(options);
    }

    /// <summary>
    /// Simula um controller com um utilizador autenticado no contexto HTTP.
    /// </summary>
    private void SimularUtilizadorAutenticado(ControllerBase controller, int userId)
    {
        var claims = new List<Claim>
        {
            new Claim("id", userId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    /// <summary>
    /// Verifica se CriarPedidoCompra cria um pedido com sucesso para um utilizador válido.
    /// </summary>
    [Fact]
    public async Task CriarPedidoCompra_DeveRetornarCreated_SeUtilizadorValido()
    {
        var context = GetInMemoryDbContext();

        
        var utilizador = new Utilizador
        {
            UtilizadorId = 1,
            PrimeiroNome = "Ana",
            Sobrenome = "Silva",
            Password = "abc123",         
            Role = "Gestor"              
        };

        context.Utilizadores.Add(utilizador);
        await context.SaveChangesAsync();

        var controller = new PedidoCompraController(context);

       
        SimularUtilizadorAutenticado(controller, 1);

        var dto = new CriarPedidoCompraDTO
        {
            Descricao = "Comprar materiais de teste"
        };

        var resultado = await controller.CriarPedidoCompra(dto);

        var created = Assert.IsType<ObjectResult>(resultado);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal("Pedido de compra criado com sucesso.", created.Value);
    }

    /// <summary>
    /// Verifica se CriarPedidoCompra retorna BadRequest quando o utilizador não existe.
    /// </summary>
    [Fact]
    public async Task CriarPedidoCompra_DeveRetornarBadRequest_SeUtilizadorNaoExistir()
    {
        var context = GetInMemoryDbContext();

        var controller = new PedidoCompraController(context);
        SimularUtilizadorAutenticado(controller, 999); // utilizador inexistente

        var dto = new CriarPedidoCompraDTO { Descricao = "Pedido inválido" };
        var resultado = await controller.CriarPedidoCompra(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal("Utilizador não encontrado.", badRequest.Value);
    }

    /// <summary>
    /// Verifica se ListarPedidosCompra devolve a lista com os nomes dos utilizadores.
    /// </summary>
    [Fact]
    public async Task ListarPedidosCompra_DeveRetornarLista()
    {
        var context = GetInMemoryDbContext();

        var user = new Utilizador
        {
            UtilizadorId = 1,
            PrimeiroNome = "Ana",
            Sobrenome = "Silva",
            Password = "123456",
            Role = "Gestor"
        };

        context.Utilizadores.Add(user);
        context.PedidosCompra.Add(new PedidoCompra
        {
            Descricao = "Pedido de teste",
            Estado = "Aberto",
            DataAbertura = DateTime.UtcNow,
            UtilizadorUtilizadorId = 1,
            UtilizadorUtilizador = user
        });
        await context.SaveChangesAsync();

        var controller = new PedidoCompraController(context);
        SimularUtilizadorAutenticado(controller, 1);

        var resultado = await controller.ListarPedidosCompra();

        var ok = Assert.IsType<OkObjectResult>(resultado.Result);
        var lista = Assert.IsAssignableFrom<IEnumerable<PedidoCompraDTO>>(ok.Value);
        Assert.Single(lista);
        Assert.Equal("Ana Silva", lista.First().NomeUtilizador); // ✅ corrigido
    }

    /// <summary>
    /// Verifica se ListarPedidosCompraPorUtilizador devolve apenas os pedidos do utilizador autenticado.
    /// </summary>
    [Fact]
    public async Task ListarPedidosCompraPorUtilizador_DeveRetornarPedidosDoUtilizador()
    {
        var context = GetInMemoryDbContext();

        var user = new Utilizador
        {
            UtilizadorId = 1,
            PrimeiroNome = "Ana",
            Sobrenome = "Silva",
            Password = "123456",
            Role = "Gestor"
        };

        context.Utilizadores.Add(user);
        context.PedidosCompra.Add(new PedidoCompra
        {
            Descricao = "Pedido de teste",
            Estado = "Aberto",
            DataAbertura = DateTime.UtcNow,
            UtilizadorUtilizadorId = 1,
            UtilizadorUtilizador = user
        });
        await context.SaveChangesAsync();

        var controller = new PedidoCompraController(context);
        SimularUtilizadorAutenticado(controller, 1);

        var resultado = await controller.ListarPedidosCompraPorUtilizador();

        var ok = Assert.IsType<OkObjectResult>(resultado.Result);
        var lista = Assert.IsAssignableFrom<IEnumerable<PedidoCompraDTO>>(ok.Value);
        Assert.Single(lista);
        Assert.Equal("Ana Silva", lista.First().NomeUtilizador); 
    }


    /// <summary>
    /// Verifica se AtualizarEstadoPedido altera o estado e define a data de conclusão.
    /// </summary>
    [Fact]
    public async Task AtualizarEstadoPedido_DeveAtualizarEstadoEDataConclusao()
    {
        var context = GetInMemoryDbContext();

        var pedido = new PedidoCompra
        {
            PedidoCompraId = 1,
            Descricao = "Pedido original",
            Estado = "Aberto",
            DataAbertura = DateTime.UtcNow
        };
        context.PedidosCompra.Add(pedido);
        await context.SaveChangesAsync();

        var controller = new PedidoCompraController(context);
        SimularUtilizadorAutenticado(controller, 1);

        var dto = new AtualizarEstadoPedidoDTO { Estado = "Aceite" };
        var resultado = await controller.AtualizarEstadoPedido(1, dto);

        var ok = Assert.IsType<OkObjectResult>(resultado);
        Assert.Equal("Estado atualizado com sucesso.", ok.Value);
        Assert.Equal("Aceite", context.PedidosCompra.Find(1)?.Estado);
        Assert.NotNull(context.PedidosCompra.Find(1)?.DataConclusao);
    }

    /// <summary>
    /// Verifica se AtualizarDescricao altera corretamente o texto da descrição.
    /// </summary>
    [Fact]
    public async Task AtualizarDescricao_DeveAtualizarDescricaoPedido()
    {
        var context = GetInMemoryDbContext();

        var pedido = new PedidoCompra
        {
            PedidoCompraId = 1,
            Descricao = "Pedido antigo",
            Estado = "Aberto",
            DataAbertura = DateTime.UtcNow
        };
        context.PedidosCompra.Add(pedido);
        await context.SaveChangesAsync();

        var controller = new PedidoCompraController(context);
        SimularUtilizadorAutenticado(controller, 1);

        var dto = new AtualizarDescricaoPedidoDTO { NovaDescricao = "Nova descrição" };
        var resultado = await controller.AtualizarDescricao(1, dto);

        var ok = Assert.IsType<OkObjectResult>(resultado);
        Assert.Equal("Descrição atualizada com sucesso.", ok.Value);
        Assert.Equal("Nova descrição", context.PedidosCompra.Find(1)?.Descricao);
    }
}
