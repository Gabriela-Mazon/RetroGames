
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _autenticacaoService;

    public AutenticacaoController(
        IAutenticacaoService autenticacaoService)
    {
        _autenticacaoService = autenticacaoService;
    }

    // POST /api/Autenticacao/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>>> Login(
        [FromBody] LoginRequestApiModel requisicao)
    {
        var usuario = await _autenticacaoService.AutenticarAsync(
            requisicao.Email,
            requisicao.Senha);

        if (usuario is null)
        {
            return Unauthorized(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.CREDENCIAIS_INVALIDAS.Code,
                ErrorMessage = ResponseCodes.CREDENCIAIS_INVALIDAS.DefaultMessage
            });
        }

        var principal = usuario.CriarPrincipal();

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        var dados = new UsuarioLogadoResponseApiModel
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = principal.FindFirst(ClaimTypes.Role)?.Value
                ?? string.Empty
        };

        return Ok(new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados
        });
    }
    // POST /api/Autenticacao/logout
[HttpPost("logout")]
[Authorize]
public async Task<ActionResult<RetroGamesResponseApiModel>> Logout()
{
    await HttpContext.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme);

    return Ok(new RetroGamesResponseApiModel
    {
        ResponseCode = ResponseCodes.SUCESSO
    });
}

    // GET /api/Autenticacao/eu
    [HttpGet("eu")]
    [Authorize]
    public ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>> Eu()
    {
        var id = User.ObterIdUsuario();

        if (id is null)
        {
            return Unauthorized(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.NAO_AUTENTICADO.Code,
                ErrorMessage = ResponseCodes.NAO_AUTENTICADO.DefaultMessage
            });
        }

        var dados = new UsuarioLogadoResponseApiModel
        {
            Id = id.Value,
            Nome = User.FindFirst(ClaimTypes.Name)?.Value
                ?? string.Empty,
            Email = User.FindFirst(ClaimTypes.Email)?.Value
                ?? string.Empty,
            Perfil = User.FindFirst(ClaimTypes.Role)?.Value
                ?? string.Empty
        };

        return Ok(
            new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = dados
            });
    }

}
