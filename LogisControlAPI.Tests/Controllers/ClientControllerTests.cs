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
/// Testes unitários para o ClienteController.
/// Valida os métodos de listagem, criação, atualização e pesquisa de clientes.
/// </summary>
public class ClienteControllerTests
{
    /// <summary>
    /// Cria uma instância do contexto da base de dados InMemory para testes isolados.
    /// </summary>
    private LogisControlContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LogisControlContext>()
            .UseInMemoryDatabase("ClienteDb_" + System.Guid.NewGuid())
            .Options;

        return new LogisControlContext(options);
    }

    /// <summary>
    /// Verifica se o método GetClientes devolve uma lista de clientes registados.
    /// </summary>
    [Fact]
    public async Task GetClientes_DeveRetornarListaDeClientes()
    {
        var context = GetInMemoryDbContext();
        context.Clientes.Add(new Cliente { Nome = "João", Nif = 123456789, Morada = "Rua A" });
        await context.SaveChangesAsync();

        var controller = new ClienteController(context);
        var resultado = await controller.GetClientes();

        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var clientes = Assert.IsAssignableFrom<IEnumerable<ClienteDTO>>(okResult.Value);
        Assert.Single(clientes);
    }

    /// <summary>
    /// Verifica se GetClientePorNif retorna um cliente existente.
    /// </summary>
    [Fact]
    public async Task GetClientePorNif_DeveRetornarCliente_SeExistir()
    {
        var context = GetInMemoryDbContext();
        context.Clientes.Add(new Cliente { Nome = "Maria", Nif = 987654321, Morada = "Rua B" });
        await context.SaveChangesAsync();

        var controller = new ClienteController(context);
        var resultado = await controller.GetClientePorNif(987654321);

        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var cliente = Assert.IsType<ClienteDTO>(okResult.Value);
        Assert.Equal("Maria", cliente.Nome);
    }

    /// <summary>
    /// Verifica se GetClientePorNif retorna NotFound quando o cliente não existe.
    /// </summary>
    [Fact]
    public async Task GetClientePorNif_DeveRetornarNotFound_SeNaoExistir()
    {
        var context = GetInMemoryDbContext();
        var controller = new ClienteController(context);

        var resultado = await controller.GetClientePorNif(999999999);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(resultado.Result);
        Assert.Equal("Cliente não encontrado.", notFoundResult.Value);
    }

    /// <summary>
    /// Verifica se CriarCliente cria um novo cliente quando os dados são válidos.
    /// </summary>
    [Fact]
    public async Task CriarCliente_DeveRetornarCreated_SeValido()
    {
        var context = GetInMemoryDbContext();
        var controller = new ClienteController(context);

        var dto = new CriarClienteDTO
        {
            Nome = "Novo Cliente",
            Nif = 111111111,
            Morada = "Rua Nova"
        };

        var resultado = await controller.CriarCliente(dto);

        var created = Assert.IsType<ObjectResult>(resultado);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal("Cliente criado com sucesso.", created.Value);
    }

    /// <summary>
    /// Verifica se CriarCliente retorna BadRequest quando o NIF já está registado.
    /// </summary>
    [Fact]
    public async Task CriarCliente_DeveRetornarBadRequest_SeNifDuplicado()
    {
        var context = GetInMemoryDbContext();
        context.Clientes.Add(new Cliente { Nome = "Duplicado", Nif = 222222222, Morada = "Rua X" });
        await context.SaveChangesAsync();

        var controller = new ClienteController(context);

        var dto = new CriarClienteDTO
        {
            Nome = "Novo Cliente",
            Nif = 222222222,
            Morada = "Rua Nova"
        };

        var resultado = await controller.CriarCliente(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal("Já existe um cliente com o mesmo NIF.", badRequest.Value);
    }

    /// <summary>
    /// Verifica se AtualizarCliente atualiza um cliente existente com dados válidos.
    /// </summary>
    [Fact]
    public async Task AtualizarCliente_DeveAtualizarERetornarOk()
    {
        var context = GetInMemoryDbContext();
        var cliente = new Cliente { Nome = "Antigo", Nif = 123456789, Morada = "Rua Antiga" };
        context.Clientes.Add(cliente);
        await context.SaveChangesAsync();

        var controller = new ClienteController(context);

        var dto = new AtualizarClienteDTO
        {
            Nome = "Atualizado",
            Nif = 987654321,
            Morada = "Rua Nova"
        };

        var resultado = await controller.AtualizarCliente(cliente.ClienteId, dto);

        var okResult = Assert.IsType<OkObjectResult>(resultado);
        Assert.Equal("Cliente atualizado com sucesso.", okResult.Value);
    }

    /// <summary>
    /// Verifica se AtualizarCliente retorna NotFound quando o cliente não existe.
    /// </summary>
    [Fact]
    public async Task AtualizarCliente_DeveRetornarNotFound_SeNaoExistir()
    {
        var context = GetInMemoryDbContext();
        var controller = new ClienteController(context);

        var dto = new AtualizarClienteDTO
        {
            Nome = "Inexistente",
            Nif = 333333333,
            Morada = "Rua X"
        };

        var resultado = await controller.AtualizarCliente(999, dto);

        var notFound = Assert.IsType<NotFoundObjectResult>(resultado);
        Assert.Equal("Cliente não encontrado.", notFound.Value);
    }

    /// <summary>
    /// Verifica se AtualizarCliente retorna BadRequest quando o novo NIF já está associado a outro cliente.
    /// </summary>
    [Fact]
    public async Task AtualizarCliente_DeveRetornarBadRequest_SeNifDuplicado()
    {
        var context = GetInMemoryDbContext();

        context.Clientes.AddRange(
            new Cliente { ClienteId = 1, Nome = "Cliente A", Nif = 123456789, Morada = "Rua A" },
            new Cliente { ClienteId = 2, Nome = "Cliente B", Nif = 987654321, Morada = "Rua B" }
        );
        await context.SaveChangesAsync();

        var controller = new ClienteController(context);

        var dto = new AtualizarClienteDTO
        {
            Nome = "Cliente Atualizado",
            Nif = 987654321, // NIF já utilizado por outro cliente
            Morada = "Rua Atualizada"
        };

        var resultado = await controller.AtualizarCliente(1, dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal("Já existe outro cliente com o mesmo NIF.", badRequest.Value);
    }
}
