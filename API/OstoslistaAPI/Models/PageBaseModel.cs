using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OstoslistaAPI.Models
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PageBaseModel : PageModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetUserEmail()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }

            return "";
        }
    }
}
