﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public IndexModel(IShoppingListService shoppingListService)
            : base(shoppingListService)
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

                if (!TryGetUserEmailIdentifierWithShopperCheck(out string emailIdentifier))
                {
                    return false;
                }

                return Shopper.FriendRequests?.Any(o => string.Equals(o.Email, emailIdentifier, StringComparison.InvariantCultureIgnoreCase)) ?? false;
            }
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
    }
}
