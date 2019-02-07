using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OstoslistaAPI.Models;
using OstoslistaData;

namespace OstoslistaAPI.Pages
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class SettingsModel : PageBaseModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shoppingListService"></param>
        /// <param name="hostingEnvironment"></param>
        public SettingsModel(IShoppingListService shoppingListService, IHostingEnvironment hostingEnvironment)
            : base(shoppingListService, hostingEnvironment)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override string ShopperName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shopperName"></param>
        public async Task<IActionResult> OnGet([FromQuery(Name = "lista")] string shopperName)
        {
            ShopperName = shopperName ?? base.ShopperName;

            if (Shopper != null)
            {
                Shopper = await _shoppingListService.GetShopper(ShopperName);
            }

            return Page();
        }
    }
}