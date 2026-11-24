using Microsoft.AspNetCore.Authorization;
using RetroGames.Model.Constantes;

namespace RetroGames.WebApi.Configuration;

public static class AutorizacaoConfig
{
    public static IServiceCollection AddAutorizacao(
        this IServiceCollection services)
    {
        services.AddAuthorization(opcoes =>
        {
            // Exigir autenticação por padrão.
            opcoes.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Exigir o perfil Administrador nas rotas de administração.
            opcoes.AddPolicy(
                Politicas.Administrador,
                politica =>
                    politica.RequireRole(Perfis.Administrador));
        });

        return services;
    }
}