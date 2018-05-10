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
                InCart = false,
                Created = DateTime.Parse("1.1.2018", CultureInfo.GetCultureInfo("fi-fi")),
                Modified = null,
            },
            new ShoppingListItemResult
            {
                Id = new Guid("003761CA-F22D-4E05-94C6-070EEA7CB653"),
                Title = "Leipää",
                InCart = true,
                Created = DateTime.Parse("2.1.2018", CultureInfo.GetCultureInfo("fi-fi")),
                Modified = DateTime.Parse("6.3.2018", CultureInfo.GetCultureInfo("fi-fi")),
            },
            new ShoppingListItemResult
            {
                Id = new Guid("7151B191-6038-46E5-871B-8E3C276FF51C"),
                Title = "Juustoa",
                InCart = false,
                Created = DateTime.Parse("10.3.2018", CultureInfo.GetCultureInfo("fi-fi")),
                Modified = null,
            },
        };

        public async Task<IEnumerable<ShoppingListItemResult>> FindShoppingListItems(Func<ShoppingListItemResult, bool> func)
        {
            lock (mockedItemList)
            {
                return mockedItemList.Where(func);
            }
        }

        public async Task<ShoppingListItemResult> CreateShoppingListItem(string title)
        {
            lock (mockedItemList)
            {
                ShoppingListItemResult retval = new ShoppingListItemResult
                {
                    Id = Guid.NewGuid(),
                    Title = title,
                    InCart = false,
                    Created = DateTime.Now
                };

                mockedItemList.Add(retval);

                return retval;
            }
        }

        public async Task DeleteShoppingListItems(Predicate<ShoppingListItemResult> match)
        {
            lock (mockedItemList)
            {
                mockedItemList.RemoveAll(match);
            }
        }
    }
}
