using Application.Common.Abstractions;
using Domain.UserAggregate.Exceptions;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Infrastructure.Dto.GetAccounts;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure;
public class TinkoffHttpClient : ITinkoffHttpClient
{
    private readonly HttpClient _client;
    private readonly string _tinkoffUrl;
    private readonly IMapper _mapper;

    public TinkoffHttpClient(HttpClient client,
                             IMapper mapper,
                             string tinkoffUrl)
    {
        _client = client;
        _tinkoffUrl = tinkoffUrl;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Portfolio>> GetPortfoliosAsync(string token, CancellationToken cancellationToken = default)
    {
        var content = new HttpRequestMessage
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json"),
            RequestUri = new Uri(_tinkoffUrl+ "/GetAccounts"),
            Method = HttpMethod.Post
        };

        content.Headers.Add("Authorization", $"Bearer {token}");

        var response = await _client.SendAsync(content, cancellationToken);

        if(response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new InvalidTokenException();
        }

        response.EnsureSuccessStatusCode();

        var serializedResponse = await response.Content.ReadFromJsonAsync<GetAccountsResponse>(cancellationToken: cancellationToken);

        return _mapper.Map<IEnumerable<Portfolio>>(serializedResponse.Accounts);
    }
}
