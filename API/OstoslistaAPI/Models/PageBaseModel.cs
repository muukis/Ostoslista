using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OstoslistaAPI.Common;
using OstoslistaData;

namespace OstoslistaAPI.Models
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PageBaseModel : PageModel
    {
        protected const string _shopperNameKey = "shopperName";
        protected readonly IShoppingListService _shoppingListService;

        /// <summary>
        /// 
        /// </summary>
        protected PageBaseModel() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shoppingListService"></param>
        protected PageBaseModel(IShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService;
        }

        /// <summary>
        /// 
        /// </summary>
        public ShopperEntity Shopper { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string ShopperName
        {
            get => HttpContext.Session.GetString(_shopperNameKey);
            set => HttpContext.Session.SetString(_shopperNameKey, value);
        }

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
        /// <returns></returns>
        public string GetUserEmail()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }

            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UserIsReadAuthenticated => Shopper != null && User.GetShopperReadAuthorization(Shopper);

        /// <summary>
        /// 
        /// </summary>
        public bool UserIsWriteAuthenticated => Shopper != null && User.GetShopperWriteAuthorization(Shopper);

        /// <summary>
        /// 
        /// </summary>
        public bool UserIsOwnerAuthenticated => Shopper != null && User.GetShopperOwnerAuthorization(Shopper);
    }
}
