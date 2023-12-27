namespace Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = WebApplication.CreateBuilder(args)
                                .AddApplication()
                                .AddInfrastructure()
                                .AddPresentation()
                                .Build();

        await app.AddMiddlewares()
                 .RunAsync();
    }
}