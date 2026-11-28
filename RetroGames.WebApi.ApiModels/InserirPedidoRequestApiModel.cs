namespace RetroGames.WebApi.ApiModels;

public class InserirPedidoRequestApiModel
{
    public List<ItemCompraRequestApiModel> Itens { get; set; } = [];
}

public class ItemCompraRequestApiModel
{
    public int IdJogo { get; set; }
    public int Quantidade { get; set; }
}
