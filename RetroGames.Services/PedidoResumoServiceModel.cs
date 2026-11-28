namespace RetroGames.Services;

public record PedidoResumoServiceModel(int Id, DateTime Data, string Situacao, decimal Total,
    List<ItemPedidoResumoServiceModel> Itens);

public record ItemPedidoResumoServiceModel(int IdJogo, string NomeJogo, int Quantidade, decimal ValorUnitario);
