using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class PlataformaMapper
{
    public static ListarPlataformaResponseApiModel ParaResponse(
        this Plataforma plataforma)
    {
        return new ListarPlataformaResponseApiModel
        {
            Id = plataforma.Id,
            Nome = plataforma.Nome,
            Fabricante = plataforma.Fabricante,
            AnoLancamento = plataforma.AnoLancamento
        };
    }
        // Converter os dados recebidos no cadastro.
    public static Plataforma ParaModel(
        this InserirPlataformaRequestApiModel requisicao)
    {
        return new Plataforma
        {
            Nome = requisicao.Nome,
            Fabricante = requisicao.Fabricante,
            AnoLancamento = requisicao.AnoLancamento
        };
    }

    // Converter os dados recebidos na alteração, usando o ID da rota.
    public static Plataforma ParaModel(
        this AlterarPlataformaRequestApiModel requisicao,
        int id)
    {
        return new Plataforma
        {
            Id = id,
            Nome = requisicao.Nome,
            Fabricante = requisicao.Fabricante,
            AnoLancamento = requisicao.AnoLancamento
        };
    }
}
