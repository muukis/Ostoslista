using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OstoslistaAPI.Pages
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        /// <summary>
        /// 
        /// </summary>
        public void OnGet()
        {
            //HttpContext.Session.Clear();
            Response.Redirect("/");
        }
    }
}