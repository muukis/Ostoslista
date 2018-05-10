using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OstoslistaContracts;

namespace OstoslistaServices
{
    public interface IShoppingListService
    {
        Task<IEnumerable<ShoppingListItemResult>> FindShoppingListItems(Func<ShoppingListItemResult, bool> func);
        Task<ShoppingListItemResult> CreateShoppingListItem(string title);
        Task DeleteShoppingListItems(Predicate<ShoppingListItemResult> match);
    }
}
