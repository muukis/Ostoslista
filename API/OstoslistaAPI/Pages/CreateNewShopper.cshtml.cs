using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OstoslistaAPI.Pages
{
    public class CreateNewShopperModel : PageModel
    {
        private static Random random = new Random();

        public void OnGet()
        {

        }

        public string GetRandomShopperName()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, random.Next(4, 6))
                .Select(s =>
                {
                    char c = s[random.Next(s.Length)];
                    return random.Next(2) == 0 ? c : c.ToString().ToLower()[0];
                }).ToArray());
        }
    }
}