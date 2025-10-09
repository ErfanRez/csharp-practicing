using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer;
using DomainLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages.Products
{
    [BindProperties]
    public class AddModel : PageModel
    {
        private ApplicationContext _context;

        public AddModel(ApplicationContext context)
        {
            _context = context;
        }


        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public string Tags { get; set; }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var product = new Product()
            {
                ProductName = ProductName,
                ImageName = ImageName,
                Description = Description,
                Tags = Tags.Split("-").ToList()
            };
            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToPage("Index");
        }
    }
}
