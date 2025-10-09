using AspMongoDB.Entities;
using AspMongoDB.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspMongoDB.Pages.Users
{
    public class DeleteModel : PageModel
    {
        private readonly IUserService _userService;

        public DeleteModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public User User { get; set; }

        public IActionResult OnGetAsync(string id)
        {

            User = _userService.GetById(id);

            if (User == null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost(string id)
        {


            _userService.Delete(id);
            return RedirectToPage("./Index");
        }
    }
}
