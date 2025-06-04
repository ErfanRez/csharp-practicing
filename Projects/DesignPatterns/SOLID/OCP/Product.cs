namespace SOLID.OCP;

public class Product
{
    public string name;
    public Color color;
    public Size size;

    public Product(string name, Color color, Size size)
    {
        this.name = name ?? throw new ArgumentNullException(paramName: nameof(name));
        this.color = color;
        this.size = size;
    }
}
