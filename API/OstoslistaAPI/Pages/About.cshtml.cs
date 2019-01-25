using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OstoslistaAPI.Models;

namespace OstoslistaAPI.Pages
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    public class AboutModel : PageBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        public void OnGet()
        {

        }
    }
}