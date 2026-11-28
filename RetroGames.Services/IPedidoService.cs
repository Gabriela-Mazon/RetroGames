using RetroGames.Model.Model;

namespace RetroGames.Services;

public record ItemCompraServiceModel(int IdJogo, int Quantidade);

public interface IPedidoService
{
    Task<List<PedidoResumoServiceModel>> ListarMeusAsync(int idUsuario);
    Task<Pedido> FinalizarAsync(int idUsuario, IReadOnlyList<ItemCompraServiceModel> carrinho);
}
