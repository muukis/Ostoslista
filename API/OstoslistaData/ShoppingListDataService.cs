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
            e.Property(o => o.Pending).IsRequired(false).ValueGeneratedOnAddOrUpdate();
            e.Property(o => o.Title).IsRequired().HasMaxLength(100);
            e.Property(o => o.Created).IsRequired(false).ValueGeneratedOnAddOrUpdate();
            e.Property(o => o.Modified).IsRequired(false).ValueGeneratedOnAddOrUpdate();
        }

        public virtual DbSet<ShoppingListItemEntity> Ostoslista { get; set; }

        public async Task<IEnumerable<IShoppingListItem>> FindItems(Expression<Func<IShoppingListItem, bool>> predicate)
        {
            return await Ostoslista.Where(predicate).ToListAsync();
        }

        public async Task<IShoppingListItem> CreateItem(string title)
        {
            var newItem = new ShoppingListItemEntity
            {
                Title = title
            };

            await Ostoslista.AddAsync(newItem);
            await SaveChangesAsync();

            return newItem;
        }

        public async Task<int> DeleteItems(Expression<Func<IShoppingListItem, bool>> predicate)
        {
            var itemsToDelete = (await FindItems(predicate)).Cast<ShoppingListItemEntity>().ToList();
            Ostoslista.RemoveRange(itemsToDelete);
            return await SaveChangesAsync();
        }

        public async Task<IShoppingListItem> Save(IShoppingListItem item = null)
        {
            await SaveChangesAsync();
            return item;
        }
    }
}
