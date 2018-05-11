using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OstoslistaContracts;

namespace OstoslistaServices
{
    public interface IShoppingListService
    {
        Task<IEnumerable<ShoppingListItemResult>> FindItems(Func<ShoppingListItemResult, bool> func);
        Task<ShoppingListItemResult> CreateItem(string title);
        Task<int> DeleteItems(Predicate<ShoppingListItemResult> match);
        Task<ShoppingListItemResult> Save(ShoppingListItemResult item);
    }
}
