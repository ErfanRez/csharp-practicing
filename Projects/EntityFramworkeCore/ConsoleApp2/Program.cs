using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer;
using DomainLayer;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            //using var context = new ApplicationContext();

            //var products = context.Products.AsTracking().ToList();
            //var product = context.Products.First();
            //product.ProductName = "123";

            //context.Products.Add(new Product()
            //{
            //    ProductName = "dasdas",
            //    ImageName = "dasdas",
            //    Description = "das"
            //});
            //Console.WriteLine(context.Entry(product).State);
            //Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
        }
    }
}
