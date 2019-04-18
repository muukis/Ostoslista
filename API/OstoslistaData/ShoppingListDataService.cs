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
            shopper.Property(o => o.ShowAdditionalButtons).IsRequired(false).ValueGeneratedOnAdd();
            shopper.Property(o => o.ApiAuthorizationBypassPassword).IsRequired(false).HasMaxLength(1024);
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

            var archivedItem = modelBuilder.Entity<ArchivedShoppingListItemEntity>();
            archivedItem.HasKey(o => o.Id);
            archivedItem.Property(o => o.Archived).IsRequired(false).ValueGeneratedOnAdd();
            archivedItem.Property(o => o.ShopperId).IsRequired();
            archivedItem.HasOne(o => o.Shopper).WithMany(o => o.ArchivedItems);
        }

        public virtual DbSet<ShopperEntity> Ostaja { get; set; }
        public virtual DbSet<ShopperFriendEntity> Kaveri { get; set; }
        public virtual DbSet<ShopperFriendRequestEntity> KaveriPyynto { get; set; }
        public virtual DbSet<ShoppingListItemEntity> Ostoslista { get; set; }
        public virtual DbSet<ArchivedShoppingListItemEntity> Arkisto { get; set; }

        public async Task<ShopperEntity> GetShopper(string shopperName)
        {
            return await Ostaja
                .Include(o => o.Friends)
                .Include(o => o.FriendRequests)
                .SingleOrDefaultAsync(o => string.Equals(o.Name, shopperName, StringComparison.InvariantCulture));
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

        public async Task<IEnumerable<ShoppingListItemEntity>> ArchiveItems(Expression<Func<ShoppingListItemEntity, bool>> predicate)
        {
            var itemsToDelete = (await FindItems(predicate)).ToList();
            var itemsToArchive = itemsToDelete.Select(o => o.CreateArchiveItem()).ToList();
            Ostoslista.RemoveRange(itemsToDelete);
            await Arkisto.AddRangeAsync(itemsToArchive);
            await SaveChangesAsync();
            return itemsToDelete;
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
            shopper.ShowAdditionalButtons = shopperSettings.ShowAdditionalButtons ?? shopper.ShowAdditionalButtons;
            shopper.ShowArchivedItems = shopperSettings.ShowArchivedItems ?? shopper.ShowArchivedItems;
            shopper.ApiAuthorizationBypassPassword = shopperSettings.ApiAuthorizationBypassPassword ?? shopper.ApiAuthorizationBypassPassword;
            shopper.Modified = DateTime.Now;
            await SaveChangesAsync();

            return shopper;
        }

        public async Task<ShopperFriendEntity> SetShopperFriendRequest(Guid shopperFriendRequestId, bool approve)
        {
            var shopperFriendRequest = await KaveriPyynto.FindAsync(shopperFriendRequestId);

            if (shopperFriendRequest == null)
            {
                throw new ArgumentException("Invalid shopper friend request identifier", nameof(shopperFriendRequestId));
            }

            ShopperFriendEntity retval = null;

            using (var tran = Database.BeginTransaction())
            {
                try
                {
                    KaveriPyynto.Remove(shopperFriendRequest);
                    
                    if (approve)
                    {
                        retval = shopperFriendRequest.ToShopperFriend();
                        await Kaveri.AddAsync(retval);
                    }

                    await SaveChangesAsync();
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }

            return retval;
        }

        public async Task<ShopperFriendEntity> DeleteShopperFriend(Guid shopperFriendId)
        {
            var shopperFriend = await Kaveri.FindAsync(shopperFriendId);

            if (shopperFriend == null)
            {
                throw new ArgumentException("Invalid shopper friend identifier", nameof(shopperFriendId));
            }

            Kaveri.Remove(shopperFriend);
            await SaveChangesAsync();
            return shopperFriend;
        }

        public async Task<ShopperFriendEntity> GetShopperFriend(Guid shopperFriendId)
        {
            return await Kaveri
                .Include(o => o.Shopper)
                .SingleOrDefaultAsync(o => o.Id == shopperFriendId);
        }

        public async Task<ShopperFriendRequestEntity> GetShopperFriendRequest(Guid shopperFriendRequestId)
        {
            return await KaveriPyynto
                .Include(o => o.Shopper)
                .SingleOrDefaultAsync(o => o.Id == shopperFriendRequestId);
        }

        public async Task<ShopperFriendRequestEntity> CreateShopperFriendRequest(Guid shopperId, string email, string name, string profileImageUrl)
        {
            var shopperFriendRequest = new ShopperFriendRequestEntity
            {
                Email = email,
                Name = name,
                ProfileImageUrl = profileImageUrl,
                ShopperId = shopperId
            };

            await KaveriPyynto.AddAsync(shopperFriendRequest);
            await SaveChangesAsync();

            return shopperFriendRequest;
        }

        public async Task<ShopperFriendRequestEntity> DeleteShopperFriendRequestByEmail(Guid shopperId, string email)
        {
            var shopper = await Ostaja.FindAsync(shopperId);
            var shopperFriendRequest = shopper.FriendRequests.SingleOrDefault(o => o.EmailMatch(email));

            if (shopperFriendRequest == null)
            {
                return null;
            }

            KaveriPyynto.Remove(shopperFriendRequest);
            await SaveChangesAsync();

            return shopperFriendRequest;
        }

        public async Task<ShopperFriendEntity> DeleteShopperFriendByEmail(Guid shopperId, string email)
        {
            var shopper = await Ostaja.FindAsync(shopperId);
            var shopperFriend = shopper.Friends.SingleOrDefault(o => o.EmailMatch(email));

            if (shopperFriend == null)
            {
                return null;
            }

            Kaveri.Remove(shopperFriend);
            await SaveChangesAsync();

            return shopperFriend;
        }

        public async Task<IEnumerable<ShopperEntity>> GetFriendRequestedShoppers(string email)
        {
            return await KaveriPyynto
                .Include(kp => kp.Shopper)
                .Where(kp => kp.EmailMatch(email))
                .Select(kp => kp.Shopper)
                .ToListAsync();
        }

        public async Task<IEnumerable<ShopperEntity>> GetFriendShoppers(string email)
        {
            return await Kaveri
                .Include(k => k.Shopper)
                .Where(k => k.EmailMatch(email))
                .Select(k => k.Shopper)
                .ToListAsync();
        }

        public async Task<IEnumerable<ShopperEntity>> GetMyShoppers(string email)
        {
            var t = await Ostaja
                .Where(o => o.EmailMatch(email))
                .ToListAsync();
            return t;
        }
    }
}
