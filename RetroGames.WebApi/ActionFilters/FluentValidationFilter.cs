using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.ActionFilters;

public class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext contexto,
        ActionExecutionDelegate proximo)
    {
        var falhas = new List<ValidationFailure>();

        foreach (var parametro in contexto.ActionDescriptor.Parameters)
        {
            contexto.ActionArguments.TryGetValue(
                parametro.Name, out var argumento);

            bool recebidoDoCorpo = parametro.BindingInfo?.BindingSource?
                .CanAcceptDataFrom(BindingSource.Body) == true;

            // Proteção extra para um modelo do corpo que chegue nulo.
            if (argumento is null)
            {
                if (recebidoDoCorpo)
                {
                    contexto.Result = new BadRequestObjectResult(
                        new RetroGamesResponseApiModel
                        {
                            ResponseCode = ResponseCodes.MODEL_NULL.Code,
                            ErrorMessage = ResponseCodes.MODEL_NULL.DefaultMessage
                        });

                    return;
                }

                // Não exigir corpo ou valor para parâmetros opcionais.
                continue;
            }

            // Procurar o validator registrado para o tipo do argumento.
            var tipoValidador = typeof(IValidator<>)
                .MakeGenericType(parametro.ParameterType);

            var validador = contexto.HttpContext.RequestServices
                .GetService(tipoValidador) as IValidator;

            if (validador is null)
            {
                continue;
            }

            // Executar as regras e reunir os erros encontrados.
            var resultado = await validador.ValidateAsync(
                new ValidationContext<object>(argumento),
                contexto.HttpContext.RequestAborted);

            falhas.AddRange(resultado.Errors);
        }

        if (falhas.Count > 0)
        {
            // Agrupar as mensagens por campo, usando nomes em camelCase.
            var errosPorCampo = falhas
                .GroupBy(falha =>
                    JsonNamingPolicy.CamelCase.ConvertName(
                        falha.PropertyName))
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo
                        .Select(falha => falha.ErrorMessage)
                        .Distinct()
                        .ToArray());

            contexto.Result = new BadRequestObjectResult(
                new RetroGamesResponseApiModel<Dictionary<string, string[]>>
                {
                    ResponseCode =
                        ResponseCodes.MODEL_STATE_INVALIDO.Code,
                    ErrorMessage =
                        ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage,
                    Data = errosPorCampo
                });

            return;
        }

        // Sem erros de validação, continuar até a action.
        await proximo();
    }
}