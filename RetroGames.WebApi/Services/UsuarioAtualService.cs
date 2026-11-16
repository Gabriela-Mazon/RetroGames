
using Microsoft.AspNetCore.Http;
using RetroGames.Model.Constantes;
using RetroGames.Services;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Services;

public class UsuarioAtualService : IUsuarioAtualService
{
    private readonly IHttpContextAccessor _contextoHttp;

    public UsuarioAtualService(IHttpContextAccessor contextoHttp)
    {
        _contextoHttp = contextoHttp;
    }

    public bool EstaAutenticado =>
        _contextoHttp.HttpContext?.User.Identity?.IsAuthenticated == true;

    public int? IdUsuario =>
        EstaAutenticado
            ? _contextoHttp.HttpContext!.User.ObterIdUsuario()
            : null;

    public bool EhAdministrador =>
        EstaAutenticado &&
        _contextoHttp.HttpContext!.User.IsInRole(Perfis.Administrador);
}
