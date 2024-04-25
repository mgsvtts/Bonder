using Bonder.Calculation.Grpc;
using Google.Protobuf.WellKnownTypes;
using Mapster;
using MapsterMapper;
using System.Reflection;
using Web.Services.Dto;

namespace Web;

public static class MapsterConfig
{
    public static IServiceCollection RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<BondFilters, Filters>
        .ForType()
        .MapWith(x => new Filters
        {
            DateFrom = x.DateFrom != null ? Timestamp.FromDateTime(new DateTime(x.DateFrom.Value, TimeOnly.MinValue, DateTimeKind.Utc)) : Timestamp.FromDateTime(DateTime.UtcNow),
            DateTo = Timestamp.FromDateTime(new DateTime(x.DateTo, TimeOnly.MinValue, DateTimeKind.Utc)),
            PriceFrom = x.PriceFrom,
            PriceTo = x.PriceTo,
            RatingFrom = x.RatingFrom,
            RatingTo = x.RatingTo,
            IncludeUnknownRatings = x.IncludeUnknownRatings
        });

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        var mapperConfig = new Mapper(typeAdapterConfig);

        services.AddSingleton<IMapper>(mapperConfig);

        return services;
    }
}
