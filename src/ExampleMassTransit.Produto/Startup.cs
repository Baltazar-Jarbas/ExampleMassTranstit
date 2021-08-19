using ExampleMassTransit.Product.Api.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using ExampleMassTransit.Product.Api.Infra.Configurations;

namespace ExampleMassTransit.Product.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddMassTransitConfig();
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("DbProduct"));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ExampleMassTransit.Product.Api", Version = "v1" });
            });
        }

        private void Seed(IApplicationBuilder app)
        {
            using var serviceSope = app.ApplicationServices.CreateScope();
            using var context = serviceSope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Products.Add(new Domain.Model.Product { Id = new System.Guid("CE44C59E-2A25-456B-80E1-9CBBD2956D17"), Name = "Refrigerante", Value = 10 });
            context.Products.Add(new Domain.Model.Product { Id = new System.Guid("CE44C59E-2A25-456B-80E1-9CBBD2956D16"), Name = "Tapioca de Queijo", Value = 20 });
            context.Products.Add(new Domain.Model.Product { Id = new System.Guid("CE44C59E-2A25-456B-80E1-9CBBD2956D15"), Name = "Tapioca de Frango", Value = 30 });
            context.SaveChanges();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExampleMassTransit.Product.Api v1"));
            }

            Seed(app);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
