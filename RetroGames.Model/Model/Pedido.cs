namespace RetroGames.Model.Model;

public class Pedido
{
    // co_seq_pedido
    public int Id { get; set; }

    // dt_pedido
    public DateTime Data { get; set; }

    // vl_total
    public decimal Total { get; set; }

    // co_usuario
    public int IdUsuario { get; set; }

    // co_situacao_pedido
    public int IdSituacaoPedido { get; set; }
}
