using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AspProMongoDb.Web.Entities;
using AspProMongoDb.Web.Services;

namespace AspProMongoDb.Web.Pages.Users
{
    public class DetailsModel : PageModel
    {
        private readonly IUserService _userService;

        public DetailsModel(IUserService userService)
        {
            _userService = userService;
        }

        public User User { get; set; }

        public IActionResult OnGet(Guid id)
        {
         
            User = _userService.GetById(id);

            if (User == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
