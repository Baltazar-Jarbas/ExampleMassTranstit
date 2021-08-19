using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using ExampleMassTransit.Client.Api.Infrastructure.Factories;
using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ExampleMassTransit.Client.Api.Infrastructure.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var ex = contextFeature.Error;

                        CustomProblemDetailsFactory problemDetailsFactory = new CustomProblemDetailsFactory();
                        string ExceptionName = ex.GetType().Name;

                        //Voce pode criar exceptions proprias da aplicação e tratalas de forma especial
                        switch (ExceptionName)
                        {
                            case nameof(TaskCanceledException):
                            case nameof(OperationCanceledException):
                                ProblemDetails operationCancelled = problemDetailsFactory.CreateProblemDetails(context, statusCode: 499, title: "Operação foi cancelada.", detail: "A operação foi cancelada pelo usuário.", instance: context.Request.Path);
                                context.Response.StatusCode = 499;
                                await ResponseAsync(context, operationCancelled);
                                break;
                            default:
                                ProblemDetails internalServerError = problemDetailsFactory.CreateProblemDetails(context, statusCode: (int)HttpStatusCode.InternalServerError, instance: context.Request.Path);
                                internalServerError.Title = "Houve um problema.";
                                internalServerError.Detail = "Não foi possível atender a solicitação no momento, por favor tente novamente mais tarde.";
                                if (env.IsDevelopment())
                                {
                                    internalServerError.Extensions.Add("Desenvolvedor", ex.ToStringDemystified());
                                }
                                context.Response.ContentType = "application/problem+json";
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                                await ResponseAsync(context, internalServerError);
                                break;
                        }

                    }
                });
            });
        }

        private static async Task ResponseAsync(HttpContext context, ProblemDetails problemDetails)
        {
            var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                WriteIndented = true,
                //Ignora propriedades com valor nulo ou padrão
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                ReferenceHandler = ReferenceHandler.Preserve
            });

            await context.Response.WriteAsync(json);
        }
    }
}
