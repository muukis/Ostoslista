using Microsoft.EntityFrameworkCore;

namespace OstoslistaData
{
    public interface IShoppingListDataService : IShoppingListService
    {
        DbSet<ShoppingListItemEntity> Ostoslista { get; set; }
        DbSet<ShopperEntity> Ostaja { get; set; }
        DbSet<ShopperFriendEntity> Kaveri { get; set; }
        DbSet<ShopperFriendRequestEntity> KaveriPyynto { get; set; }
    }
}