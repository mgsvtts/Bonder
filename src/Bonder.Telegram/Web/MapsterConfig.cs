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
        .MapWith(x => CustomMappings.MapFilters(x));

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        var mapperConfig = new Mapper(typeAdapterConfig);

        services.AddSingleton<IMapper>(mapperConfig);

        return services;
    }
}

public static class CustomMappings
{
    public static Filters MapFilters(BondFilters source)
    {
        var type = source.DateToType switch
        {
            DateToType.Custom => GrpcDateIntervalType.Custom,
            DateToType.Maturity => GrpcDateIntervalType.Maturity,
            DateToType.Offer => GrpcDateIntervalType.Offer,
            _ => throw new NotImplementedException(),
        };

        return new Filters
        {
            DateIntervalType = type,
            DateFrom = source.DateFrom != null ? Timestamp.FromDateTime(new DateTime(source.DateFrom.Value, TimeOnly.MinValue, DateTimeKind.Utc)) : Timestamp.FromDateTime(DateTime.UtcNow),
            DateTo = Timestamp.FromDateTime(new DateTime(source.DateTo, TimeOnly.MinValue, DateTimeKind.Utc)),
            PriceFrom = source.PriceFrom,
            PriceTo = source.PriceTo,
            RatingFrom = source.RatingFrom,
            RatingTo = source.RatingTo,
            IncludeUnknownRatings = source.IncludeUnknownRatings
        };
    }
}