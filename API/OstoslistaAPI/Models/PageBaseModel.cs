using System;
using System.Web;
using Microsoft.AspNetCore.Hosting;
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
        /// <summary>
        /// 
        /// </summary>
        protected const string _shopperNameKey = "shopperName";
        /// <summary>
        /// 
        /// </summary>
        protected readonly IShoppingListService _shoppingListService;
        /// <summary>
        /// 
        /// </summary>
        protected readonly IHostingEnvironment _hostingEnvironment;

        /// <summary>
        /// 
        /// </summary>
        protected PageBaseModel() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shoppingListService"></param>
        /// <param name="hostingEnvironment"></param>
        protected PageBaseModel(IShoppingListService shoppingListService, IHostingEnvironment hostingEnvironment)
        {
            _shoppingListService = shoppingListService;
            _hostingEnvironment = hostingEnvironment;
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

        /// <summary>
        /// 
        /// </summary>
        public string MailTo => (Shopper == null ? null : $"mailto:?subject={Uri.EscapeUriString($"Ostoslistan linkki")}&body={Uri.EscapeUriString($"Hei,\n\nMennään yhdessä kauppareissulle!\n\nTässä on linkki ostoslistaan:\n{SiteRootUrl}?lista={Uri.EscapeUriString(Shopper.Name)}\n\n")}");

        /// <summary>
        /// 
        /// </summary>
        public string SiteRootUrl => $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetBase64EncodedHtmlEmelentImageSource(string url)
        {
            var qrImage = new QRImage();
            return $"data:image/png;base64,{qrImage.CreateBase64EncodedQRImageFromUrl(url)}";
        }
    }
}
