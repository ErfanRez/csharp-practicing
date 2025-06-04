using SOLID.OCP;

namespace ConsoleApp;

public class Program
{
    static void Main(string[] args)
    {

        // Single Responsibility:

        //Journal journal = new Journal();
        //journal.add("I went to college");
        //journal.add("I ate ice cream");

        //Console.WriteLine(journal);

        //Persistence p = new Persistence();
        //string filename = Path.GetFullPath(@"C:\Users\ERFAN\Desktop\C# Practicing\Projects\DesignPatterns\journal.txt");

        //Console.WriteLine(filename);


        //p.Save(filename, journal);

        //Process.Start(new ProcessStartInfo
        //{
        //    FileName = filename,
        //    UseShellExecute = true // This is required to open .txt files
        //});

        //*******************************************************************************************/

        // Open for extension but closed for modification:

        Product apple = new Product("Apple", Color.Red, Size.Small);
        Product shoes = new Product("Shoes", Color.Black, Size.Medium);
        Product phone = new Product("Phone", Color.Blue, Size.Large);

        Product[] products = { apple, shoes, phone };

        var filter = new FilterProducts();

        foreach (var item in filter.Filter(products, new ColorSpecification(Color.Red)))
        {
            Console.WriteLine($"{item.name} is red");
        }

        foreach (var item in filter.Filter(products, new AndSpecification<Product>(
            new SizeSpecification(Size.Medium),
            new ColorSpecification(Color.Black))))
        {
            Console.WriteLine($"{item.name} is black and medium.");
        }


    }
}
