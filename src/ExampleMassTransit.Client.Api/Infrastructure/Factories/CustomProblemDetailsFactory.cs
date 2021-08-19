using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Net;

namespace ExampleMassTransit.Client.Api.Infrastructure.Factories
{
    public class CustomProblemDetailsFactory : ProblemDetailsFactory
    {
        private readonly ApiBehaviorOptions _options;
        public CustomProblemDetailsFactory()
        {

        }
        public CustomProblemDetailsFactory(IOptions<ApiBehaviorOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }
        public override ProblemDetails CreateProblemDetails(HttpContext httpContext, int? statusCode = null, string title = null, string type = null, string detail = null, string instance = null)
        {
            statusCode ??= httpContext.Response.StatusCode;

            ProblemDetails problemDetails = new ProblemDetails

            {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = instance,

            };

            UseCustomProblemDetails(httpContext, problemDetails, statusCode.GetValueOrDefault(httpContext.Response.StatusCode));

            return problemDetails;
        }

        public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, ModelStateDictionary modelStateDictionary, int? statusCode = null, string title = null, string type = null, string detail = null, string instance = null)
        {
            if (modelStateDictionary == null)
            {
                throw new ArgumentNullException(nameof(modelStateDictionary));
            }



            statusCode ??= 400;



            ValidationProblemDetails problemDetails = new ValidationProblemDetails(modelStateDictionary)
            {

                Status = statusCode,
                Type = type,
                Detail = detail ?? "Favor verificar a propriedade `errors` para mais detalhes.",
                Instance = instance,
            };

            if (title != null)
            {
                problemDetails.Title = title;
            }

            UseCustomProblemDetails(httpContext, problemDetails, statusCode.Value);

            return problemDetails;
        }

        private void UseCustomProblemDetails(HttpContext httpContext, ProblemDetails problemDetails, int statusCode)
        {

            problemDetails.Status ??= statusCode;
            problemDetails.Title = DescricaoStatusCode(statusCode);
            problemDetails.Instance = httpContext.Request.Path;

            if (_options != null && _options.ClientErrorMapping.TryGetValue(statusCode, out ClientErrorData clientErrorData))
            {
                problemDetails.Title ??= clientErrorData.Title;
            }

            if(Enum.IsDefined(typeof(HttpStatusCode), problemDetails.Status))
            {
                problemDetails.Type = $"https://developer.mozilla.org/pt-BR/docs/Web/HTTP/Status/{statusCode}";
            }
            else
            {
                problemDetails.Type = "Não documentado";
            }
             
            string traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;

            if (traceId != null)
            {
                problemDetails.Extensions["traceId"] = traceId;
            }

        }

        private static string DescricaoStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 204:
                    return "Não há conteúdo para enviar para esta solicitação.";
                case 301:
                    return "URI do recurso requerido mudou.";
                case 400:
                    return "Um ou mais erros de validação ocorreram.";
                case 401:
                    return "O cliente deve se autenticar para obter a resposta solicitada.";
                case 403:
                    return "O cliente não tem direitos de acesso ao conteúdo portanto o servidor está rejeitando dar a resposta.";
                case 404:
                    return "O servidor não pode encontrar o recurso solicitado.";
                case 405:
                    return "O método de solicitação é conhecido pelo servidor, mas em outro verbo.";
                case 407:
                    return "É necessário que a autenticação seja feita por um proxy";
                case 408:
                    return "Conexão ociosa";
                case 501:
                    return "O recurso da requisição não é suportado pelo servidor e não pode ser manipulado.";
                case 503:
                    return "O servidor não está pronto para manipular a requisição.";

                default:
                    return null;

            }
        }
    }
}
