namespace SOLID.OCP;

public class AndSpecification<T>(ISpecification<T> sp1, ISpecification<T> sp2) : ISpecification<T>
{
    private readonly ISpecification<T> _sp1 = sp1 ?? throw new ArgumentNullException(paramName: nameof(sp1)), _sp2 = sp2 ?? throw new ArgumentNullException(paramName: nameof(sp2));

    public bool IsSatisfied(T t)
    {
        return _sp1.IsSatisfied(t) && _sp2.IsSatisfied(t);
    }
}