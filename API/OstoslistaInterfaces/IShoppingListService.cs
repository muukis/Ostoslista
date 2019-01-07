using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OstoslistaInterfaces
{
    public interface IShoppingListService
    {
        Task<IEnumerable<IShoppingListItem>> FindItems(Expression<Func<IShoppingListItem, bool>> predicate);
        Task<IShoppingListItem> CreateItem(string title);
        Task<int> DeleteItems(Expression<Func<IShoppingListItem, bool>> predicate);
        Task<IShoppingListItem> Save(IShoppingListItem item = null);
    }
}
