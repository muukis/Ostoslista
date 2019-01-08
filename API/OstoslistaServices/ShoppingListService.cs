﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using OstoslistaData;
using OstoslistaInterfaces;

namespace OstoslistaServices
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly IShoppingListDataService _dataService;

        public ShoppingListService(IShoppingListDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IEnumerable<IShoppingListItem>> FindItems(Expression<Func<IShoppingListItem, bool>> predicate)
        {
            return await _dataService.FindItems(predicate);
        }

        public async Task<IShoppingListItem> CreateItem(string title)
        {
            return await _dataService.CreateItem(title);
        }

        public async Task<IShoppingListItem> UpdateItemPendingStatus(Guid id, bool isPending)
        {
            return await _dataService.UpdateItemPendingStatus(id, isPending);
        }

        public async Task<int> DeleteItems(Expression<Func<IShoppingListItem, bool>> predicate)
        {
            return await _dataService.DeleteItems(predicate);
        }
    }
}
