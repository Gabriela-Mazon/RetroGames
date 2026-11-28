using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface ISituacaoPedidoRepository
{
    Task<List<SituacaoPedido>> ListarAsync();
}
