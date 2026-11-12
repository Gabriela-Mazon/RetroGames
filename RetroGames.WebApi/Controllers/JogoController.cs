
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _jogoService;

    public JogoController(IJogoService jogoService)
    {
        _jogoService = jogoService;
    }

    // GET /api/Jogo
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>>> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        var filtro = request.ParaFiltro();

        var (jogos, total) = await _jogoService.ListarAsync(
            busca: filtro.Busca,
            idPlataforma: filtro.IdPlataforma,
            idGenero: filtro.IdGenero,
            pagina: filtro.Pagina,
            tamanhoPagina: filtro.TamanhoPagina,
            incluirInativos: false
        );

        var pagina = new PaginaResponseApiModel<ListarJogoResponseApiModel>
        {
            Itens = jogos
                .Select(j => j.ParaListagemResponse())
                .ToList(),
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
            TotalItens = total,
            TotalPaginas = total == 0
                ? 0
                : (int)Math.Ceiling((double)total / filtro.TamanhoPagina)
        };

        return Ok(
            new RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = pagina
            });
    }

    // GET /api/Jogo/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> ListarPorId(
        int id)
    {
        var jogo = await _jogoService.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = jogo.ParaDetalheResponse()
        });
    }
}
