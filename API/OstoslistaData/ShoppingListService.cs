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

        public async Task<IEnumerable<ShoppingListItemEntity>> DeleteItems(Expression<Func<ShoppingListItemEntity, bool>> predicate)
        {
            return await _dataService.DeleteItems(predicate);
        }
    }
}
