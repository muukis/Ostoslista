using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using OstoslistaAPI.Models;

namespace OstoslistaAPI.Pages
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    public class LogoutModel : PageBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public async void OnGet()
        {
            await HttpContext.SignOutAsync();
            Response.Redirect("/");
        }
    }
}