using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer;
using DomainLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages.Users
{
    [BindProperties]
    public class AddModel : PageModel
    {
        private readonly ApplicationContext _context;

        public AddModel(ApplicationContext context)
        {
            _context = context;
        }


        public string Name { get; set; }
        public string Family { get; set; }
        public string Email { get; set; }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var user = new User()
            {
                Email = Email,
                Name = Name,
                Family = Family,
                CreationDate = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToPage("Index");
        }
    }
}
