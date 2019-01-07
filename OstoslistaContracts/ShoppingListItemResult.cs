﻿using System;
using OstoslistaInterfaces;

namespace OstoslistaContracts
{
    public class ShoppingListItemResult : IShoppingListItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool? Pending { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
