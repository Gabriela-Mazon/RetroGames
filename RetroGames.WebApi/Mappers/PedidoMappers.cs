using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class PedidoMappers
{
    public static ListarMeusPedidosResponseApiModel ParaResposta(this PedidoResumoServiceModel pedido) =>
        new(pedido.Id, pedido.Data, pedido.Situacao, pedido.Total, pedido.Itens.Select(item =>
            new ItemPedidoResponseApiModel(item.IdJogo, item.NomeJogo, item.Quantidade, item.ValorUnitario)).ToList());
}
