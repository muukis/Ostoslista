using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OstoslistaAPI.Models;
using OstoslistaData;

namespace OstoslistaAPI.Pages
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    public class IndexModel : PageBaseModel
    {
        private const string _shopperNameKey = "shopperName";
        private readonly IShoppingListService _shoppingListService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shoppingListService"></param>
        public IndexModel(IShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        /// <summary>
        /// 
        /// </summary>
        public ShopperEntity Shopper { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string ShopperName => HttpContext.Session.GetString(_shopperNameKey);

        /// <summary>
        /// 
        /// </summary>
        public string UrlEncodedShopperName => HttpUtility.UrlEncode(ShopperName);

        /// <summary>
        /// 
        /// </summary>
        public string HtmlEncodedShopperName => HttpUtility.HtmlEncode(ShopperName);

        /// <summary>
        /// 
        /// </summary>
        public string EscapedShopperName => (ShopperName ?? string.Empty).Replace(@"\", @"\\").Replace("\"", "\\\"");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shopperName"></param>
        public async Task<IActionResult> OnGet([FromQuery(Name = "lista")] string shopperName)
        {
            if (shopperName != null)
            {
                HttpContext.Session.SetString(_shopperNameKey, shopperName);
            }

            if (ShopperName != null)
            {
                Shopper = await _shoppingListService.GetShopper(ShopperName);
            }

            return Page();
        }
    }
}
