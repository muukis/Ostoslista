using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OstoslistaAPI.Pages
{
    /// <summary>
    /// 
    /// </summary>
    public class IndexModel : PageModel
    {
        private const string shopperNameKey = "shopperName";

        /// <summary>
        /// 
        /// </summary>
        public string ShopperName => HttpContext.Session.GetString(shopperNameKey);

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
        public void OnGet([FromQuery(Name = "lista")] string shopperName)
        {
            if (shopperName != null)
            {
                HttpContext.Session.SetString(shopperNameKey, shopperName);
            }
        }
    }
}
