using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OstoslistaAPI.Pages
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    public class CreateNewShopperModel : PageModel
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// 
        /// </summary>
        public void OnGet()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetRandomShopperName()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, _random.Next(4, 6))
                .Select(s =>
                {
                    char c = s[_random.Next(s.Length)];
                    return _random.Next(2) == 0 ? c : c.ToString().ToLower()[0];
                }).ToArray());
        }
    }
}