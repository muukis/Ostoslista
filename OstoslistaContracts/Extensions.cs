using System;
using System.Collections.Generic;
using System.Linq;
using OstoslistaData;

namespace OstoslistaContracts
{
    public static class Extensions
    {
        public static IEnumerable<ShoppingListItemResult> ToResults(this IEnumerable<ShoppingListItemEntity> items)
        {
            return items.Select(o => o.ToResult());
        }

        public static ShoppingListItemResult ToResult(this ShoppingListItemEntity item)
        {
            return new ShoppingListItemResult
            {
                Id = item.Id ?? Guid.Empty,
                Title = item.Title,
                Pending = item.Pending ?? true,
                ShopperName = item.Shopper.Name
            };
        }

        public static ShopperSettings ToDataObject(this SetShopperSettingsDto settings, Guid? shopperId)
        {
            return new ShopperSettings
            {
                Id = shopperId,
                AllowNewFriendRequests = settings.AllowNewFriendRequests,
                PublicReadAccess = settings.PublicReadAccess,
                PublicWriteAccess = settings.PublicWriteAccess,
                FriendReadAccess = settings.FriendReadAccess,
                FriendWriteAccess = settings.FriendWriteAccess
            };
        }

        public static GetShopperSettingsResult ToSettingsResult(this ShopperEntity shopper)
        {
            return new GetShopperSettingsResult
            {
                AllowNewFriendRequests = shopper.AllowNewFriendRequests ?? false,
                PublicReadAccess = shopper.PublicReadAccess ?? false,
                PublicWriteAccess = shopper.PublicWriteAccess ?? false,
                FriendReadAccess = shopper.FriendReadAccess ?? false,
                FriendWriteAccess = shopper.FriendWriteAccess ?? false
            };
        }
    }
}
