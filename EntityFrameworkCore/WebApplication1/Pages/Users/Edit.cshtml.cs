using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages.Users
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ApplicationContext _context;

        public EditModel(ApplicationContext context)
        {
            _context = context;
        }


        public string Name { get; set; }
        public string Family { get; set; }
        public string Email { get; set; }
        public IActionResult OnGet(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return RedirectToPage("Index");

            Name = user.Name;
            Family = user.Family;
            Email = user.Email;
            return Page();
        }
        public IActionResult OnPost(int id)
        {
            var user = _context.Users.First(u => u.Id == id);
            user.Email = Email;
            user.Family = Family;
            user.Name = Name;

            _context.Users.Update(user);
            _context.SaveChanges();
            return RedirectToPage("Index");
        }

    }
}
