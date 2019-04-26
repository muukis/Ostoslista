using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OstoslistaData
{
    public interface IShoppingListService
    {
        Task<ShopperEntity> GetShopper(string shopperName);
        Task<ShopperEntity> CreateShopper(string shopperName, string emailIdentifier);
        Task<IEnumerable<ShoppingListItemEntity>> FindItems(Expression<Func<ShoppingListItemEntity, bool>> predicate);
        Task<IEnumerable<ArchivedShoppingListItemEntity>> FindArchivedItems(Expression<Func<ArchivedShoppingListItemEntity, bool>> predicate);
        Task<ShoppingListItemEntity> CreateItem(string shopperName, string title);
        Task<ShoppingListItemEntity> UpdateItemPendingStatus(Guid id, bool isPending);
        Task<IEnumerable<Tuple<ShoppingListItemEntity, ArchivedShoppingListItemEntity>>> ArchiveItems(Expression<Func<ShoppingListItemEntity, bool>> predicate);
        Task<IEnumerable<ShoppingListItemEntity>> DeleteItems(Expression<Func<ShoppingListItemEntity, bool>> predicate);
        Task<IEnumerable<ArchivedShoppingListItemEntity>> DeleteArchivedItems(Expression<Func<ArchivedShoppingListItemEntity, bool>> predicate);
        Task<ShopperEntity> SaveShopperSettings(ShopperSettings shopperSettings);
        Task<ShopperFriendEntity> SetShopperFriendRequest(Guid shopperFriendRequestId, bool approve);
        Task<ShopperFriendEntity> DeleteShopperFriend(Guid shopperFriendId);
        Task<ShopperFriendEntity> GetShopperFriend(Guid shopperFriendId);
        Task<ShopperFriendRequestEntity> GetShopperFriendRequest(Guid shopperFriendRequestId);
        Task<ShopperFriendRequestEntity> CreateShopperFriendRequest(Guid shopperId, string emailIdentifier, string name, string profileImageUrl);
        Task<ShopperFriendRequestEntity> DeleteShopperFriendRequestByEmail(Guid shopperId, string emailIdentifier);
        Task<ShopperFriendEntity> DeleteShopperFriendByEmail(Guid shopperId, string emailIdentifier);
        Task<IEnumerable<ShopperEntity>> GetMyShoppers(string emailIdentifier);
        Task<IEnumerable<ShopperEntity>> GetFriendRequestedShoppers(string emailIdentifier);
        Task<IEnumerable<ShopperEntity>> GetFriendShoppers(string emailIdentifier);
    }
}
