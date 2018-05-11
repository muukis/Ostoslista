using OstoslistaContracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OstoslistaServices
{
    /// <summary>
    /// 
    /// </summary>
    public class ShoppingListServiceMock : IShoppingListService
    {
        private static List<ShoppingListItemResult> mockedItemList = new List<ShoppingListItemResult>
        {
            new ShoppingListItemResult
            {
                Id = new Guid("193F1DFC-416D-49DC-BE81-C7CB6F72FEA4"),
                Title = "Maitoa",
                Pending = false,
                Created = DateTime.Parse("1.1.2018", CultureInfo.GetCultureInfo("fi-fi")),
                Modified = null,
            },
            new ShoppingListItemResult
            {
                Id = new Guid("003761CA-F22D-4E05-94C6-070EEA7CB653"),
                Title = "Leipää",
                Pending = true,
                Created = DateTime.Parse("2.1.2018", CultureInfo.GetCultureInfo("fi-fi")),
                Modified = DateTime.Parse("6.3.2018", CultureInfo.GetCultureInfo("fi-fi")),
            },
            new ShoppingListItemResult
            {
                Id = new Guid("7151B191-6038-46E5-871B-8E3C276FF51C"),
                Title = "Juustoa",
                Pending = false,
                Created = DateTime.Parse("10.3.2018", CultureInfo.GetCultureInfo("fi-fi")),
                Modified = null,
            },
        };

        public async Task<IEnumerable<ShoppingListItemResult>> FindItems(Func<ShoppingListItemResult, bool> func)
        {
            lock (mockedItemList)
            {
                return mockedItemList.Where(func);
            }
        }

        public async Task<ShoppingListItemResult> CreateItem(string title)
        {
            lock (mockedItemList)
            {
                var retval = new ShoppingListItemResult
                {
                    Id = Guid.NewGuid(),
                    Title = title,
                    Pending = false,
                    Created = DateTime.Now
                };

                mockedItemList.Add(retval);
                return retval;
            }
        }

        public async Task<int> DeleteItems(Predicate<ShoppingListItemResult> match)
        {
            lock (mockedItemList)
            {
                return mockedItemList.RemoveAll(match);
            }
        }

        public async Task<ShoppingListItemResult> Save(ShoppingListItemResult item)
        {
            return item;
        }
    }
}
