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
    public class IndexModel : PageModel
    {
        private readonly ApplicationContext _context;

        public IndexModel(ApplicationContext context)
        {
            _context = context;
        }

        public List<Product> Products { get; set; }
        public void OnGet()
        {
            Products = _context.Products.OrderByDescending(d => d.Id).ToList();
        }

        public IActionResult OnPost(int id)
        {
            var product = _context.Products.FirstOrDefault(f => f.Id == id);
            if (product == null)
                return RedirectToPage("Index");


            _context.Remove(product);
            _context.SaveChanges();
            return RedirectToPage("Index");
        }
    }
}
