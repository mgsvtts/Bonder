namespace Web;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var app = WebApplication.CreateBuilder(args)
        .AddDomain()
        .AddApplication()
        .AddInfrastructure()
        .AddPresentation()
        .Build();

        await app.AddMiddlewares()
                 .RunAsync();
    }
}