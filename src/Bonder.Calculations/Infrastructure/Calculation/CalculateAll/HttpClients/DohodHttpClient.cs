using Application.Commands.Calculation.Common.Abstractions;
using Domain.BondAggreagte.ValueObjects.Identities;
using Infrastructure.Calculation.Dto.GetRating;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http.Json;

namespace Infrastructure.Calculation.CalculateAll.HttpClients;

public sealed class DohodHttpClient : IDohodHttpClient
{
    private readonly HttpClient _client;
    private readonly string _serverUrl;

    public DohodHttpClient(HttpClient client, string serverUrl)
    {
        _client = client;
        _serverUrl = serverUrl;
    }

    public async Task<int?> GetBondRatingAsync(Isin isin, CancellationToken token = default)
    {
        var content = new HttpRequestMessage
        {
            RequestUri = new Uri(BuildQuery(isin)),
            Method = HttpMethod.Get
        };

        var response = await _client.SendAsync(content, token);

        response.EnsureSuccessStatusCode();

        var serializedResponse = await response.Content.ReadFromJsonAsync<IEnumerable<DohodItem>>(cancellationToken: token)
                                 ?? throw new InvalidOperationException("Ошибка получения ответа от dohod.ru");

        var item = serializedResponse.FirstOrDefault(x => x?.Isin?.ToUpper() == isin.Value);

        return item?.Rating != null ? int.Parse(item.Rating) : null;
    }

    private string BuildQuery(Isin isin)
    {
        var query = new Dictionary<string, string>
        {
            ["action"] = "replacement",
            ["isin"] = isin.Value,
            ["mode"] = "regular"
        };

        return _serverUrl + new QueryBuilder(query);
    }
}