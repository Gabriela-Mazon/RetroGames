
using RetroGames.Model.Constantes;

namespace RetroGames.WebApi.Configuration;

public static class AutorizacaoConfig
{
    public static IServiceCollection AddAutorizacao(
        this IServiceCollection services)
    {
        services.AddAuthorization(opcoes =>
        {
            opcoes.AddPolicy(
                Politicas.Administrador,
                politica =>
                    politica.RequireRole(Perfis.Administrador));
        });

        return services;
    }
}
