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

        public static T ToResult<T>(this BaseShopperFriendEntity shopperFriend)
            where T : BaseShopperFriendResult, new()
        {
            return new T
            {
                Id = shopperFriend.Id ?? Guid.Empty,
                Name = shopperFriend.Name,
                ProfileImageUrl = shopperFriend.ProfileImageUrl
            };
        }

        public static IEnumerable<T> ToResults<T>(this IEnumerable<BaseShopperFriendEntity> shopperFriends)
            where T : BaseShopperFriendResult, new()
        {
            return shopperFriends.Select(o => o.ToResult<T>());
        }

        public static ShopperResult ToResult(this ShopperEntity shopper)
        {
            return new ShopperResult
            {
                ShopperName = shopper.Name,
                AllowNewFriends = shopper.AllowNewFriendRequests ?? false,
                PublicReadAccess = shopper.PublicReadAccess ?? false,
                PublicWriteAccess = shopper.PublicWriteAccess ?? false,
                FriendReadAccess = shopper.FriendReadAccess ?? false,
                FriendWriteAccess = shopper.FriendWriteAccess ?? false,
                ItemCount = shopper.Items.Count,
                FriendRequestCount = shopper.FriendRequests.Count,
                FriendCount = shopper.Friends.Count,
            };
        }

        public static IEnumerable<ShopperResult> ToResults(this IEnumerable<ShopperEntity> shoppers)
        {
            return shoppers.Select(o => o.ToResult());
        }
    }
}
