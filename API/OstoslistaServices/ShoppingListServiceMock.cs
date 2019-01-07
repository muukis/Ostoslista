using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OstoslistaData;
using OstoslistaInterfaces;

namespace OstoslistaServices
{
    /// <summary>
    /// 
    /// </summary>
    public class ShoppingListServiceMock : IShoppingListService
    {
        private static List<ShoppingListItemEntity> mockedItemList = new List<ShoppingListItemEntity>
        {
            new ShoppingListItemEntity
            {
                Id = new Guid("193F1DFC-416D-49DC-BE81-C7CB6F72FEA4"),
                Title = "Maitoa",
                Pending = false,
                Created = DateTime.Parse("1.1.2018", CultureInfo.GetCultureInfo("fi-fi")),
                Modified = null,
            },
            new ShoppingListItemEntity
            {
                Id = new Guid("003761CA-F22D-4E05-94C6-070EEA7CB653"),
                Title = "Leipää",
                Pending = true,
                Created = DateTime.Parse("2.1.2018", CultureInfo.GetCultureInfo("fi-fi")),
                Modified = DateTime.Parse("6.3.2018", CultureInfo.GetCultureInfo("fi-fi")),
            },
            new ShoppingListItemEntity
            {
                Id = new Guid("7151B191-6038-46E5-871B-8E3C276FF51C"),
                Title = "Juustoa",
                Pending = false,
                Created = DateTime.Parse("10.3.2018", CultureInfo.GetCultureInfo("fi-fi")),
                Modified = null,
            },
        };

        private static IQueryable<ShoppingListItemEntity> mockedItemQueryableList = null;

        private static IQueryable<ShoppingListItemEntity> GetQueryableList()
        {
            if (mockedItemQueryableList == null)
            {
                mockedItemQueryableList = mockedItemList.AsQueryable();
            }

            return mockedItemQueryableList;
        }

        public async Task<IEnumerable<IShoppingListItem>> FindItems(Expression<Func<IShoppingListItem, bool>> predicate)
        {
            return GetQueryableList().Where(predicate).ToList();
        }

        public async Task<IShoppingListItem> CreateItem(string title)
        {
            var retval = new ShoppingListItemEntity
            {
                Id = Guid.NewGuid(),
                Title = title,
                Pending = false,
                Created = DateTime.Now
            };

            mockedItemList.Add(retval);
            mockedItemQueryableList = null;

            return retval;
        }

        public async Task<int> DeleteItems(Expression<Func<IShoppingListItem, bool>> predicate)
        {
            var itemsToDelete = (await FindItems(predicate)).Cast<ShoppingListItemEntity>().ToList();
            itemsToDelete.ForEach(o => mockedItemList.Remove(o));
            mockedItemQueryableList = null;
            return itemsToDelete.Count;
        }

        public async Task<IShoppingListItem> Save(IShoppingListItem item = null)
        {
            return item;
        }
    }
}
