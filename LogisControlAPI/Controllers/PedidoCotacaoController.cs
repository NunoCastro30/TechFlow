using LogisControlAPI.DTO;
using LogisControlAPI.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/pedidos-cotacao")]
public class PedidoCotacaoController : ControllerBase
{
    private readonly ComprasService _service;
    public PedidoCotacaoController(ComprasService service) => _service = service;

    /// <summary>
    /// Cria um pedido de cotação para um pedido de compra existente,
    /// atribuindo-o a um fornecedor e gera um token de acesso.
    /// </summary>
    [HttpPost("{pedidoCompraId}/cotacao")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CriarPedidoCotacao(
        [FromRoute] int pedidoCompraId,
        [FromQuery] int fornecedorId)
    {
        Console.WriteLine("Entrou em CriarPedidoCotacao");
        try
        {
            var (cotacaoId, token) = await _service
                .CriarPedidoCotacaoAsync(pedidoCompraId, fornecedorId);

            // 201 Created + Location (incluindo token) + body com token
            return CreatedAtAction(
                nameof(ObterPorId),
                new { id = cotacaoId, token = token },   // <-- adiciona o token aqui
                new
                {
                    PedidoCotacaoId = cotacaoId,
                    TokenAcesso = token
                }
            );
        }
        catch (KeyNotFoundException knf)
        {
            return NotFound(knf.Message);
        }
        catch (InvalidOperationException io)
        {
            return BadRequest(io.Message);
        }
    }

    /// <summary>
    /// Retorna o pedido de cotação com seus orçamentos e itens.
    /// </summary>
    // GET /api/pedidos-cotacao/{id}?token=XYZ
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PedidoCotacaoDetalhadoDTO), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ObterPorId(
    [FromRoute] int id,
    [FromQuery] string token)
    {
        try
        {
            var dto = await _service.ObterPedidoCotacaoParaFornecedorAsync(id, token);
            return Ok(dto);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Token inválido");
        }
    }
}