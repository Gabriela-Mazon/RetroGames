
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static ClaimsPrincipal CriarPrincipal(this Usuario usuario)
    {
        var perfil = usuario.IdPerfil == PerfilIds.Administrador
            ? Perfis.Administrador
            : Perfis.Cliente;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, perfil)
        };

        var identidade = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        return new ClaimsPrincipal(identidade);
    }

    public static int? ObterIdUsuario(this ClaimsPrincipal principal)
    {
        var valor = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(valor, out var id))
        {
            return id;
        }

        return null;
    }
}
