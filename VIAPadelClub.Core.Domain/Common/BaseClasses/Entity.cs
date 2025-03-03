namespace VIAPadelClub.Core.Domain.Common.BaseClasses;

public class Entity
{
    public Guid Id { get; set; }

    protected Entity(Guid id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var other = (Entity)obj;
        return Id == other.Id;
    }

    public override string ToString()
    {
        return base.ToString();
    }
}