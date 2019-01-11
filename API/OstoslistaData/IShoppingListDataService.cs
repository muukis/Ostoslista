using Microsoft.EntityFrameworkCore;
using OstoslistaInterfaces;

namespace OstoslistaData
{
    public interface IShoppingListDataService : IShoppingListService
    {
        DbSet<ShoppingListItemEntity> Ostoslista { get; set; }
        DbSet<ShopperEntity> Ostaja { get; set; }
    }
}