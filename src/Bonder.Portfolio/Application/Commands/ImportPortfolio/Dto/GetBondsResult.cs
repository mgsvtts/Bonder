using Bonder.Calculation.Grpc;

namespace Application.Commands.ImportPortfolio.Dto;

public readonly record struct GetBondsResult(IList<GrpcBond> OperationBonds, Dictionary<GrpcBond, int> Bonds);