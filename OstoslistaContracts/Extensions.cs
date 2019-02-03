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

        public static T ToResult<T>(this ShopperEntity shopper, Action<T> propertySetter = null)
            where T : BaseShoppersResult, new()
        {
            var retval = new T
            {
                ShopperName = shopper.Name,
            };

            propertySetter?.Invoke(retval);
            return retval;
        }

        public static IEnumerable<T> ToResults<T>(this IEnumerable<ShopperEntity> shoppers, Action<T> propertySetter = null)
            where T : BaseShoppersResult, new()
        {
            return shoppers.Select(o => o.ToResult(propertySetter));
        }

        public static FriendShoppersResult ToFriendShoppersResult(this ShopperEntity shopper)
        {
            return shopper.ToResult<FriendShoppersResult>(o =>
            {
                o.ItemCount = shopper.Items.Count;
            });
        }

        public static IEnumerable<FriendShoppersResult> ToFriendShoppersResult(this IEnumerable<ShopperEntity> shoppers)
        {
            return shoppers.Select(o => o.ToFriendShoppersResult());
        }

        public static MyShopperResult ToMyShopperResult(this ShopperEntity shopper)
        {
            return shopper.ToResult<MyShopperResult>(o =>
            {
                o.ItemCount = shopper.Items.Count;
                o.AllowNewFriends = shopper.AllowNewFriendRequests ?? false;
                o.PublicReadAccess = shopper.PublicReadAccess ?? false;
                o.PublicWriteAccess = shopper.PublicWriteAccess ?? false;
                o.FriendReadAccess = shopper.FriendReadAccess ?? false;
                o.FriendWriteAccess = shopper.FriendWriteAccess ?? false;
                o.FriendRequestCount = shopper.FriendRequests.Count;
                o.FriendCount = shopper.Friends.Count;
            });
        }

        public static IEnumerable<MyShopperResult> ToMyShopperResults(this IEnumerable<ShopperEntity> shoppers)
        {
            return shoppers.Select(o => o.ToMyShopperResult());
        }
    }
}
