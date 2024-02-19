using Application.Common.Abstractions;
using Domain.UserAggregate.Exceptions;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Infrastructure.Dto.GetAccounts;
using MapsterMapper;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Linq;
using Application.Common.Abstractions.Dto;

namespace Infrastructure;

public class TinkoffHttpClient : ITinkoffHttpClient
{
    private readonly HttpClient _client;
    private readonly string _tinkoffUserServiceUrl;
    private readonly string _tinkoffOperationsServiceUrl;
    private readonly IMapper _mapper;

    public TinkoffHttpClient(HttpClient client,
                             IMapper mapper,
                             string tinkoffUrl,
                             string tinkoffOperationsServiceUrl)
    {
        _client = client;
        _tinkoffUserServiceUrl = tinkoffUrl;
        _mapper = mapper;
        _tinkoffOperationsServiceUrl = tinkoffOperationsServiceUrl;
    }

    public async Task<List<Portfolio>> GetPortfoliosAsync(string token, CancellationToken cancellationToken = default)
    {
        var content = new HttpRequestMessage
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json"),
            RequestUri = new Uri(_tinkoffUserServiceUrl + "/GetAccounts"),
            Method = HttpMethod.Post
        };

        content.Headers.Add("Authorization", $"Bearer {token}");

        var response = await _client.SendAsync(content, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new InvalidTokenException();
        }

        response.EnsureSuccessStatusCode();

        var serializedResponse = await response.Content.ReadFromJsonAsync<GetAccountsResponse>(cancellationToken: cancellationToken);

        var tasks = serializedResponse.Accounts.Select(account => GetPortfolioAsync(new GetPortfoliosRequest(account.Id, token)));

        await Task.WhenAll(tasks);

        return tasks.Select(x => x.Result).ToList();
    }

    private async Task<Portfolio> GetPortfolioAsync(GetPortfoliosRequest request, CancellationToken cancellationToken = default)
    {
        var content = new HttpRequestMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(new { account_id = request.AccountId, currency = "RUB" }), Encoding.UTF8, "application/json"),
            RequestUri = new Uri(_tinkoffOperationsServiceUrl + "/GetPortfolio"),
            Method = HttpMethod.Post
        };

        content.Headers.Add("Authorization", $"Bearer {request.Token}");
        var response = await _client.SendAsync(content, cancellationToken);

        response.EnsureSuccessStatusCode();

        var serializedResponse = await response.Content.ReadFromJsonAsync<GetTinkoffPortfolioResponse>(cancellationToken: cancellationToken);

        return _mapper.Map<Portfolio>(serializedResponse);
    }
}
