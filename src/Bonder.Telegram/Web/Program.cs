
using Application;
using Bonder.Calculation.Grpc;

namespace Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddGrpc();

        var portfolioServerUrl = new Uri(builder.Configuration.GetValue<string>("CalculationServerUrl"));
        builder.Services.AddGrpcClient<CalculationService.CalculationServiceClient>(options => options.Address = portfolioServerUrl);

        builder.Services.AddSingleton(x => new Bot(builder.Configuration.GetValue<string>("TelegramKey"),
                                                   x.GetRequiredService<IServiceScopeFactory>()));



        var app = builder.Build();

        app.Services.GetRequiredService<Bot>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}
