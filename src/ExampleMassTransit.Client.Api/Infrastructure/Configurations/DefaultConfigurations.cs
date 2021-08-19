using ExampleMassTransit.Client.Api.Infrastructure.CustomAttributes;
using ExampleMassTransit.Client.Api.Infrastructure.Extensions;
using ExampleMassTransit.Client.Api.Infrastructure.Factories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Text.Json;

namespace ExampleMassTransit.Client.Api.Infrastructure.Configurations
{
    public static class DefaultConfigurations
    {
        public static IServiceCollection AddDefaultConfigurations(this IServiceCollection services)
        {
            services.AddControllers(options => {
                options.Filters.Add(new ValidateModelStateAttribute());
            });

            // Diminui o tamanho do payload enviado mas atrasa um pouco o retorno
            //services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            //services.AddResponseCompression();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mock API", Version = "v1" });
            });

            services.AddSingleton<ProblemDetailsFactory, CustomProblemDetailsFactory>();
            return services;
        }

        public static IApplicationBuilder UseDefaultConfigurations(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseGlobalExceptionHandler(env);

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mock API v1"));

            

            app.UseStatusCodePages(async context => {
                ProblemDetails problemDetail = new CustomProblemDetailsFactory().CreateProblemDetails(context.HttpContext);
                context.HttpContext.Response.ContentType = "application/problem+json; charset=utf-8";
                await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(problemDetail));
            });
 
            app.UseRouting();
            app.UseAuthorization();
    

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

         

            return app;
        }
    }
}
