namespace SOLID.OCP;

public class ColorSpecification : ISpecification<Product>
{
    private readonly Color _color;

    public ColorSpecification(Color color)
    {
        _color = color;
    }

    public bool IsSatisfied(Product t)
    {
        return t.color == _color;
    }
}
