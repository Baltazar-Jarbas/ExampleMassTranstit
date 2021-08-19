using static ExampleMassTransit.Client.Api.Infrastructure.Configurations.CustomWebHost;
namespace ExampleMassTransit.Client.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateCustomHostBuilder<Startup>(args).BuildAndRun();
        }
    }
}
