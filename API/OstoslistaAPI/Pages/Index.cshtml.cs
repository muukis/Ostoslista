using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OstoslistaAPI.Common;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shoppingListService"></param>
        /// <param name="hostingEnvironment"></param>
        public IndexModel(IShoppingListService shoppingListService, IHostingEnvironment hostingEnvironment)
            : base(shoppingListService, hostingEnvironment)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shopperName"></param>
        public async Task<IActionResult> OnGet([FromQuery(Name = "lista")] string shopperName)
        {
            if (shopperName != null)
            {
                ShopperName = shopperName;
            }

            if (ShopperName != null)
            {
                Shopper = await _shoppingListService.GetShopper(ShopperName);
            }

            return Page();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UserIsShopperFriend
        {
            get
            {
                if (!TryGetUserEmailIdentifierWithShopperCheck(out string emailIdentifier))
                {
                    return false;
                }

                return Shopper.Friends?.Any(o => string.Equals(o.Email, emailIdentifier, StringComparison.InvariantCultureIgnoreCase)) ?? false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool UserIsShopperFriendRequested
        {
            get
            {
                if (UserIsShopperFriend)
                {
                    return true;
                }

                return TryGetUserShopperFriendRequestId(out Guid temp);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shopperFriendId"></param>
        /// <returns></returns>
        public bool TryGetUserShopperFriendRequestId(out Guid shopperFriendId)
        {
            if (!TryGetUserEmailIdentifierWithShopperCheck(out string emailIdentifier))
            {
                shopperFriendId = Guid.Empty;
                return false;
            }

            var shopperFriendRequest =
                Shopper.FriendRequests?.SingleOrDefault(o => string.Equals(o.Email, emailIdentifier, StringComparison.InvariantCultureIgnoreCase));

            shopperFriendId = shopperFriendRequest?.Id ?? Guid.Empty;

            return shopperFriendId != Guid.Empty;
        }

        private bool TryGetUserEmailIdentifierWithShopperCheck(out string emailIdentifier)
        {
            emailIdentifier = null;

            if (Shopper == null)
            {
                return false;
            }

            emailIdentifier = User.GetUserEmailIdentifier();

            if (emailIdentifier == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ShopperRootUrl => $"{SiteRootUrl}?lista={Uri.EscapeUriString(Shopper?.Name)}";
    }
}
