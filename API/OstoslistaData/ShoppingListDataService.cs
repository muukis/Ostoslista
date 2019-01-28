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
            shopper.Property(o => o.Email).IsRequired(false).HasMaxLength(100);
            shopper.Property(o => o.AllowNewFriendRequests).IsRequired(false).ValueGeneratedOnAdd();
            shopper.Property(o => o.PublicWriteAccess).IsRequired(false).ValueGeneratedOnAdd();
            shopper.Property(o => o.PublicReadAccess).IsRequired(false).ValueGeneratedOnAdd();
            shopper.Property(o => o.FriendWriteAccess).IsRequired(false).ValueGeneratedOnAdd();
            shopper.Property(o => o.FriendReadAccess).IsRequired(false).ValueGeneratedOnAdd();
            shopper.HasMany(o => o.Items).WithOne(o => o.Shopper);
            shopper.HasMany(o => o.Friends).WithOne(o => o.Shopper);
            shopper.HasMany(o => o.FriendRequests).WithOne(o => o.Shopper);

            var shopperFriend = modelBuilder.Entity<ShopperFriendEntity>();
            shopperFriend.InitBaseShopperFriendEntity(o => o.Friends);

            var shopperFriendRequest = modelBuilder.Entity<ShopperFriendRequestEntity>();
            shopperFriendRequest.InitBaseShopperFriendEntity(o => o.FriendRequests);

            var item = modelBuilder.Entity<ShoppingListItemEntity>();
            item.InitBaseShopperChildEntity(o => o.Items);
            item.Property(o => o.Pending).IsRequired(false).ValueGeneratedOnAdd();
            item.Property(o => o.Title).IsRequired().HasMaxLength(100);
        }

        public virtual DbSet<ShopperEntity> Ostaja { get; set; }
        public virtual DbSet<ShopperFriendEntity> Kaveri { get; set; }
        public virtual DbSet<ShopperFriendRequestEntity> KaveriPyynto { get; set; }
        public virtual DbSet<ShoppingListItemEntity> Ostoslista { get; set; }

        public async Task<ShopperEntity> GetShopper(string shopperName)
        {
            return await Ostaja.SingleOrDefaultAsync(o => string.Equals(o.Name, shopperName, StringComparison.InvariantCulture));
        }

        public async Task<ShopperEntity> CreateShopper(string shopperName, string emailIdentifier)
        {
            var shopper = new ShopperEntity
            {
                Name = shopperName.Trim(),
                Email = emailIdentifier
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

        public async Task<IEnumerable<ShoppingListItemEntity>> DeleteItems(Expression<Func<ShoppingListItemEntity, bool>> predicate)
        {
            var itemsToDelete = (await FindItems(predicate)).ToList();
            Ostoslista.RemoveRange(itemsToDelete);
            await SaveChangesAsync();
            return itemsToDelete;
        }

        public async Task<ShopperEntity> SaveShopperSettings(ShopperSettings shopperSettings)
        {
            var shopper = await Ostaja.FindAsync(shopperSettings.Id);

            if (shopper == null)
            {
                throw new ArgumentException("Invalid shopper identifier", nameof(shopperSettings));
            }

            shopper.AllowNewFriendRequests = shopperSettings.AllowNewFriendRequests ?? shopper.AllowNewFriendRequests;
            shopper.PublicReadAccess = shopperSettings.PublicReadAccess ?? shopper.PublicReadAccess;
            shopper.PublicWriteAccess = shopperSettings.PublicWriteAccess ?? shopper.PublicWriteAccess;
            shopper.FriendReadAccess = shopperSettings.FriendReadAccess ?? shopper.FriendReadAccess;
            shopper.FriendWriteAccess = shopperSettings.FriendWriteAccess ?? shopper.FriendWriteAccess;
            await SaveChangesAsync();

            return shopper;
        }
    }
}
