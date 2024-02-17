namespace Domain.Common.Models;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    public TId Identity { get; }

    protected Entity(TId id)
    {
        Identity = id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> entity && Identity.Equals(entity.Identity);
    }

    public bool Equals(Entity<TId>? other)
    {
        return Equals(other as object);
    }

    public static bool operator ==(Entity<TId> left, Entity<TId> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TId> left, Entity<TId> right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return Identity.GetHashCode();
    }
}