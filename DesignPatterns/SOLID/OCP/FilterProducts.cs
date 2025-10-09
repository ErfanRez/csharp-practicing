

namespace SOLID.OCP;

public class FilterProducts : IFilter<Product>
{
    public IEnumerable<Product> Filter(IEnumerable<Product> items, ISpecification<Product> spec)
    {
        foreach (var item in items)
        {
            if (spec.IsSatisfied(item))
            {
                yield return item;
            }
        }
    }
}


//Wrong approach:

//public class ProductFilter
//{
//    // let's suppose we don't want ad-hoc queries on products
//    public IEnumerable<Product> FilterByColor(IEnumerable<Product> products, Color color)
//    {
//        foreach (var p in products)
//            if (p.Color == color)
//                yield return p;
//    }

//    public static IEnumerable<Product> FilterBySize(IEnumerable<Product> products, Size size)
//    {
//        foreach (var p in products)
//            if (p.Size == size)
//                yield return p;
//    }

//    public static IEnumerable<Product> FilterBySizeAndColor(IEnumerable<Product> products, Size size, Color color)
//    {
//        foreach (var p in products)
//            if (p.Size == size && p.Color == color)
//                yield return p;
//    } // state space explosion
//      // 3 criteria = 7 methods

//    // OCP = open for extension but closed for modification
//}

