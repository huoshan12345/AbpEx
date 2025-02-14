using System;

namespace LightInject;

public class ServiceRegistrationEqualityComparer : IEqualityComparer<ServiceRegistration>
{
    public bool Equals(ServiceRegistration? x, ServiceRegistration? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x == null || y == null) return false;

        return x.ServiceType == y.ServiceType
               && x.Lifetime?.GetType() == y.Lifetime?.GetType()
               && x.ImplementingType == y.ImplementingType
               && x.Value == y.Value
               && x.FactoryExpression == y.FactoryExpression;
    }

    public int GetHashCode(ServiceRegistration obj)
    {
        var hash = new HashCode();
        hash.Add(obj.ServiceType);
        hash.Add(obj.Lifetime?.GetType());
        hash.Add(obj.ImplementingType);
        hash.Add(obj.Value);
        hash.Add(obj.FactoryExpression);
        return hash.ToHashCode();
    }

    public static ServiceRegistrationEqualityComparer Instance { get; } = new();
}