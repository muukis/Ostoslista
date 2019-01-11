using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OstoslistaData
{
    public class ShoppingListDataService : DbContext, IShoppingListDataService
    {
        public ShoppingListDataService(DbContextOptions options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var shopper = modelBuilder.Entity<ShopperEntity>();
            shopper.InitBaseEntity();
            shopper.Property(o => o.Name).IsRequired().HasMaxLength(50);
            shopper.HasMany(o => o.Items).WithOne(o => o.Shopper);

            var item = modelBuilder.Entity<ShoppingListItemEntity>();
            item.InitBaseEntity();
            item.Property(o => o.Pending).IsRequired(false).ValueGeneratedOnAdd();
            item.Property(o => o.Title).IsRequired().HasMaxLength(100);
            item.Property(o => o.ShopperId).IsRequired();
            item.HasOne(o => o.Shopper).WithMany(o => o.Items);
        }

        public virtual DbSet<ShopperEntity> Ostaja { get; set; }
        public virtual DbSet<ShoppingListItemEntity> Ostoslista { get; set; }

        public async Task<ShopperEntity> GetShopper(string shopperName)
        {
            return await Ostaja.SingleAsync(o => string.Equals(o.Name, shopperName, StringComparison.InvariantCulture));
        }

        public async Task<ShopperEntity> CreateShopper(string shopperName)
        {
            var shopper = new ShopperEntity
            {
                Name = shopperName.Trim(),
                Email = "?"
            };

            await Ostaja.AddAsync(shopper);
            await SaveChangesAsync();

            return shopper;
        }

        public async Task<IEnumerable<ShoppingListItemEntity>> FindItems(Expression<Func<ShoppingListItemEntity, bool>> predicate)
        {
            return await Ostoslista.Include(o => o.Shopper).Where(predicate).ToListAsync();
        }

        public async Task<ShoppingListItemEntity> CreateItem(string shopperName, string title)
        {
            var shopper = await GetShopper(shopperName);
            var sanitizedTitle = title.Substring(0, 1).ToUpper();

            if (title.Length > 1)
            {
                sanitizedTitle += title.Substring(1).ToLower();
            }

            var newItem = new ShoppingListItemEntity
            {
                Title = sanitizedTitle,
                ShopperId = shopper.Id ?? Guid.Empty
            };

            await Ostoslista.AddAsync(newItem);
            await SaveChangesAsync();

            return newItem;
        }

        public async Task<ShoppingListItemEntity> UpdateItemPendingStatus(Guid id, bool isPending)
        {
            var searchResult = (await FindItems(o => o.Id == id)).ToList();

            if (!searchResult.Any())
            {
                return null;
            }

            var shoppingListItem = searchResult.First();

            shoppingListItem.Pending = isPending;
            shoppingListItem.Modified = DateTime.Now;
            await SaveChangesAsync();

            return shoppingListItem;
        }

        public async Task<int> DeleteItems(Expression<Func<ShoppingListItemEntity, bool>> predicate)
        {
            var itemsToDelete = (await FindItems(predicate)).ToList();
            Ostoslista.RemoveRange(itemsToDelete);
            return await SaveChangesAsync();
        }
    }
}
