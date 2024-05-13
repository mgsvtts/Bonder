using Bonder.Calculation.Grpc;

namespace Application.Commands.Portfolios.ImportPortfolio.Dto;

public readonly record struct GetBondsResult(IList<GrpcBond> OperationBonds, Dictionary<GrpcBond, int> Bonds);