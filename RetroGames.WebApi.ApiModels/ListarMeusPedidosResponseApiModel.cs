namespace RetroGames.WebApi.ApiModels;

public record ListarMeusPedidosResponseApiModel(int Id, DateTime Data, string Situacao, decimal Total,
    List<ItemPedidoResponseApiModel> Itens);

public record ItemPedidoResponseApiModel(int IdJogo, string NomeJogo, int Quantidade, decimal ValorUnitario);
