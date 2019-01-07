using System;

namespace OstoslistaInterfaces
{
    public interface IShoppingListItem
    {
        DateTime? Created { get; set; }
        Guid? Id { get; set; }
        DateTime? Modified { get; set; }
        bool? Pending { get; set; }
        string Title { get; set; }
    }
}