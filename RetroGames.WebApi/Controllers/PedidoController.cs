using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoController(IPedidoService pedidos) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<InserirPedidoResponseApiModel>>> Finalizar(
        [FromBody] InserirPedidoRequestApiModel requisicao)
    {
        if (User.ObterIdUsuario() is not int idUsuario)
            return SessaoNecessaria();

        var itens = requisicao.Itens.Select(i => new ItemCompraServiceModel(i.IdJogo, i.Quantidade)).ToList();
        var pedido = await pedidos.FinalizarAsync(idUsuario, itens);
        return StatusCode(StatusCodes.Status201Created, new RetroGamesResponseApiModel<InserirPedidoResponseApiModel>
        {
            Data = new(pedido.Id, pedido.Data, pedido.Total, pedido.IdSituacaoPedido)
        });
    }

    [HttpGet("meus")]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarMeusPedidosResponseApiModel>>>> Meus()
    {
        if (User.ObterIdUsuario() is not int idUsuario)
            return SessaoNecessaria();

        var historico = await pedidos.ListarMeusAsync(idUsuario);
        return Ok(new RetroGamesResponseApiModel<List<ListarMeusPedidosResponseApiModel>>
        {
            Data = historico.Select(p => p.ParaResposta()).ToList()
        });
    }

    private UnauthorizedObjectResult SessaoNecessaria() =>
        Unauthorized(new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.NAO_AUTENTICADO.Code,
            ErrorMessage = ResponseCodes.NAO_AUTENTICADO.DefaultMessage
        });
}
