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
        Task<ShoppingListItemEntity> CreateItem(string shopperName, string title);
        Task<ShoppingListItemEntity> UpdateItemPendingStatus(Guid id, bool isPending);
        Task<IEnumerable<ShoppingListItemEntity>> DeleteItems(Expression<Func<ShoppingListItemEntity, bool>> predicate);
        Task<ShopperEntity> SaveShopperSettings(ShopperSettings shopperSettings);
    }
}
