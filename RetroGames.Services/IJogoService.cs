using RetroGames.Model.Model;

namespace RetroGames.Services;

public interface IJogoService
{
    Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? busca = null,
        int? idPlataforma = null,
        int? idGenero = null,
        int pagina = 1,
        int tamanhoPagina = 10,
        bool incluirInativos = false);

    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);

    Task<Jogo> InserirAsync(Jogo jogo, List<int> idsGeneros);

    Task AlterarAsync(Jogo jogo, List<int> idsGeneros);

    Task DesativarAsync(int id);
}