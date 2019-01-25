using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OstoslistaAPI.Pages
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class LoginModel : PageModel
    {
        /// <summary>
        /// 
        /// </summary>
        public void OnGet()
        {
            Response.Redirect("/");
        }
    }
}