
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    // Converter os filtros recebidos pela API.
    public static (
        string? Busca,
        int? IdPlataforma,
        int? IdGenero,
        int Pagina,
        int TamanhoPagina
    ) ParaFiltro(this ListarJogoRequestApiModel request)
    {
        return (
            request.Busca,
            request.IdPlataforma,
            request.IdGenero,
            request.Pagina,
            request.TamanhoPagina
        );
    }

    // Converter um jogo para a listagem do catálogo.
    public static ListarJogoResponseApiModel ParaListagemResponse(
        this JogoDetalheServiceModel detalhe)
    {
        var jogo = detalhe.Jogo;

        return new ListarJogoResponseApiModel
        {
            Id = jogo.Id,
            Nome = jogo.Nome,
            AnoLancamento = jogo.AnoLancamento,
            Preco = jogo.Preco,
            Estoque = jogo.Estoque,
            Ativo = jogo.Ativo,
            NomePlataforma = detalhe.NomePlataforma,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos
                .Select(g => g.Nome)
                .ToList()
        };
    }

    // Converter um jogo para a consulta detalhada.
    public static ListarPorIdJogoResponseApiModel ParaDetalheResponse(
        this JogoDetalheServiceModel detalhe)
    {
        var jogo = detalhe.Jogo;

        return new ListarPorIdJogoResponseApiModel
        {
            Id = jogo.Id,
            Nome = jogo.Nome,
            Descricao = jogo.Descricao,
            AnoLancamento = jogo.AnoLancamento,
            Preco = jogo.Preco,
            Estoque = jogo.Estoque,
            Ativo = jogo.Ativo,
            IdPlataforma = jogo.IdPlataforma,
            NomePlataforma = detalhe.NomePlataforma,
            IdCondicaoJogo = jogo.IdCondicaoJogo,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos
                .Select(g => new ListarGeneroResponseApiModel
                {
                    Id = g.Id,
                    Nome = g.Nome
                })
                .ToList()
        };
    }
}
