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
    public class IndexModel : PageModel
    {
        private readonly ApplicationContext _context;


        public IndexModel(ApplicationContext context)
        {
            _context = context;
        }

        public List<User> Users { get; set; }
        public void OnGet()
        {
            Users = _context.Users.OrderByDescending(d => d.Id).ToList();
        }

        public IActionResult OnPost(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return RedirectToPage("Index");


            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToPage("Index");
        }
    }
}
