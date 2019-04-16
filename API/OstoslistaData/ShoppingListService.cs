using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OstoslistaData
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly IShoppingListDataService _dataService;

        public ShoppingListService(IShoppingListDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<ShopperEntity> GetShopper(string shopperName)
        {
            return await _dataService.GetShopper(shopperName);
        }

        public async Task<ShopperEntity> CreateShopper(string shopperName, string emailIdentifier)
        {
            return await _dataService.CreateShopper(shopperName, emailIdentifier);
        }

        public async Task<IEnumerable<ShoppingListItemEntity>> FindItems(Expression<Func<ShoppingListItemEntity, bool>> predicate)
        {
            return await _dataService.FindItems(predicate);
        }

        public async Task<ShoppingListItemEntity> CreateItem(string shopperName, string title)
        {
            return await _dataService.CreateItem(shopperName, title);
        }

        public async Task<ShoppingListItemEntity> UpdateItemPendingStatus(Guid id, bool isPending)
        {
            return await _dataService.UpdateItemPendingStatus(id, isPending);
        }

        public async Task<IEnumerable<ShoppingListItemEntity>> ArchiveItems(Expression<Func<ShoppingListItemEntity, bool>> predicate)
        {
            return await _dataService.ArchiveItems(predicate);
        }

        public async Task<IEnumerable<ShoppingListItemEntity>> DeleteItems(Expression<Func<ShoppingListItemEntity, bool>> predicate)
        {
            return await _dataService.DeleteItems(predicate);
        }

        public async Task<ShopperEntity> SaveShopperSettings(ShopperSettings shopperSettings)
        {
            return await _dataService.SaveShopperSettings(shopperSettings);
        }

        public async Task<ShopperFriendEntity> SetShopperFriendRequest(Guid shopperFriendRequestId, bool approve)
        {
            return await _dataService.SetShopperFriendRequest(shopperFriendRequestId, approve);
        }

        public async Task<ShopperFriendEntity> DeleteShopperFriend(Guid shopperFriendId)
        {
            return await _dataService.DeleteShopperFriend(shopperFriendId);
        }

        public async Task<ShopperFriendEntity> GetShopperFriend(Guid shopperFriendId)
        {
            return await _dataService.GetShopperFriend(shopperFriendId);
        }

        public async Task<ShopperFriendRequestEntity> GetShopperFriendRequest(Guid shopperFriendRequestId)
        {
            return await _dataService.GetShopperFriendRequest(shopperFriendRequestId);
        }

        public async Task<ShopperFriendRequestEntity> CreateShopperFriendRequest(Guid shopperId, string email, string name, string profileImageUrl)
        {
            return await _dataService.CreateShopperFriendRequest(shopperId, email, name, profileImageUrl);
        }

        public async Task<ShopperFriendRequestEntity> DeleteShopperFriendRequestByEmail(Guid shopperId, string email)
        {
            return await _dataService.DeleteShopperFriendRequestByEmail(shopperId, email);
        }

        public async Task<ShopperFriendEntity> DeleteShopperFriendByEmail(Guid shopperId, string email)
        {
            return await _dataService.DeleteShopperFriendByEmail(shopperId, email);
        }

        public async Task<IEnumerable<ShopperEntity>> GetMyShoppers(string email)
        {
            return await _dataService.GetMyShoppers(email);
        }

        public async Task<IEnumerable<ShopperEntity>> GetFriendRequestedShoppers(string email)
        {
            return await _dataService.GetFriendRequestedShoppers(email);
        }

        public async Task<IEnumerable<ShopperEntity>> GetFriendShoppers(string email)
        {
            return await _dataService.GetFriendShoppers(email);
        }
    }
}
