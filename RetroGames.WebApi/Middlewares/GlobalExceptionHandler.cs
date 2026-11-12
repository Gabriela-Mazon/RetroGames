
using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext contexto,
        Exception excecao,
        CancellationToken cancellationToken)
    {
        // Tratar erros de negócio.
        if (excecao is BusinessRuleException erroNegocio)
        {
            contexto.Response.StatusCode =
                (int)erroNegocio.Erro.Status;

            await contexto.Response.WriteAsJsonAsync(
                new RetroGamesResponseApiModel
                {
                    ResponseCode = erroNegocio.Erro.Code,
                    ErrorMessage = erroNegocio.Message
                },
                cancellationToken: cancellationToken);

            return true;
        }

        // Registrar erros inesperados no log.
        _logger.LogError(
            excecao,
            "Erro inesperado na API RetroGames.");

        // Não mostrar detalhes internos ao usuário.
        contexto.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        await contexto.Response.WriteAsJsonAsync(
            new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.ERRO,
                ErrorMessage =
                    "Ocorreu um erro interno. Tente novamente mais tarde."
            },
            cancellationToken: cancellationToken);

        return true;
    }
}
