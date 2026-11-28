using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IPedidoRepository
{
    Task<int> InserirAsync(Pedido pedido);
}
