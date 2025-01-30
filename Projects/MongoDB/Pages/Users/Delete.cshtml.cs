using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AspProMongoDb.Web.Entities;
using AspProMongoDb.Web.Services;

namespace AspProMongoDb.Web.Pages.Users
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

        public IActionResult OnGetAsync(Guid id)
        {

            User = _userService.GetById(id);

            if (User == null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost(Guid id)
        {
         

            _userService.Delete(id);
            return RedirectToPage("./Index");
        }
    }
}
