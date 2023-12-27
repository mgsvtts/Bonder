using Application.Calculation.Common.CalculationService;
using Application.Calculation.Common.Interfaces;
using Application.Common;
using Infrastructure.Calculation;
using MapsterMapper;
using Presentation.Middlewares;
using Web.Extensions.Mapping;

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