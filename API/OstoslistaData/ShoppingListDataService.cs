using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OstoslistaInterfaces;

namespace OstoslistaData
{
    public class ShoppingListDataService : DbContext, IShoppingListDataService
    {
        public ShoppingListDataService(DbContextOptions options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var e = modelBuilder.Entity<ShoppingListItemEntity>();

            e.HasKey(o => o.Id);
            e.Property(o => o.Pending).IsRequired(false).ValueGeneratedOnAdd();
            e.Property(o => o.Title).IsRequired().HasMaxLength(100);
            e.Property(o => o.Created).IsRequired(false).ValueGeneratedOnAdd();
            e.Property(o => o.Modified).IsRequired(false).ValueGeneratedOnAdd();
        }

        public virtual DbSet<ShoppingListItemEntity> Ostoslista { get; set; }

        public async Task<IEnumerable<IShoppingListItem>> FindItems(Expression<Func<IShoppingListItem, bool>> predicate)
        {
            return await Ostoslista.Where(predicate).ToListAsync();
        }

        public async Task<IShoppingListItem> CreateItem(string title)
        {
            var sanitizedTitle = title.Substring(0, 1).ToUpper();

            if (title.Length > 1)
            {
                sanitizedTitle += title.Substring(1).ToLower();
            }

            var newItem = new ShoppingListItemEntity
            {
                Title = sanitizedTitle
            };

            await Ostoslista.AddAsync(newItem);
            await SaveChangesAsync();

            return newItem;
        }

        public async Task<IShoppingListItem> UpdateItemPendingStatus(Guid id, bool isPending)
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

        public async Task<int> DeleteItems(Expression<Func<IShoppingListItem, bool>> predicate)
        {
            var itemsToDelete = (await FindItems(predicate)).Cast<ShoppingListItemEntity>().ToList();
            Ostoslista.RemoveRange(itemsToDelete);
            return await SaveChangesAsync();
        }
    }
}
