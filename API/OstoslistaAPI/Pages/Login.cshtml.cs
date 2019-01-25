using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using OstoslistaAPI.Models;

namespace OstoslistaAPI.Pages
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class LoginModel : PageBaseModel
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