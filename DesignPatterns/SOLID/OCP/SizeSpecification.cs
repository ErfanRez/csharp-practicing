namespace SOLID.OCP;

public class SizeSpecification(Size size) : ISpecification<Product>
{
    public bool IsSatisfied(Product t)
    {
        return t.size == size;
    }
}
