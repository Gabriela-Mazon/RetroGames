using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IItemPedidoRepository
{
    Task InserirAsync(ItemPedido item);
}
