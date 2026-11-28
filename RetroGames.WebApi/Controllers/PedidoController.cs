using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;

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

    private UnauthorizedObjectResult SessaoNecessaria() =>
        Unauthorized(new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.NAO_AUTENTICADO.Code,
            ErrorMessage = ResponseCodes.NAO_AUTENTICADO.DefaultMessage
        });
}
