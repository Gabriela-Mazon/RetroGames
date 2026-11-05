using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoGeneroRepository
{
    Task<List<JogoGenero>> ListarTodosAsync();

    Task<List<JogoGenero>> ListarPorJogosAsync(List<int> idsJogos);

    Task InserirAsync(int idJogo, List<int> idsGeneros);

    Task RemoverPorJogoAsync(int idJogo);
}