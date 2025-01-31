using AspMongoDB.Entities;
using AspMongoDB.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspMongoDB.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public List<User> Users { get; set; }

        public void OnGet()
        {
            Users = _userService.GetAll();
        }
    }
}
