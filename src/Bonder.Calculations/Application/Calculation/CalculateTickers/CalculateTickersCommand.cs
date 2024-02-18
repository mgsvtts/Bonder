using Application.Calculation.CalculateAll.Services.Dto;
using Domain.BondAggreagte.Abstractions.Dto;
using Domain.BondAggreagte.ValueObjects;
using MediatR;

namespace Application.Calculation.CalculateTickers;
public sealed record CalculateTickersCommand(GetPriceSortedRequest Options, IEnumerable<Ticker> Tickers) : IRequest<CalculateAllResponse>;