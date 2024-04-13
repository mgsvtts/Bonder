namespace Bonder.Protos.Custom;

public partial class GrpcGuid
{
    public GrpcGuid(string value)
    {
        Value = value;
    }

    public static implicit operator Guid(GrpcGuid grpcGuid)
    {
        return Guid.Parse(grpcGuid.Value);
    }

    public static implicit operator GrpcGuid(Guid value)
    {
        return new GrpcGuid(value);
    }
}