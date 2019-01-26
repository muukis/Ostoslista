﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OstoslistaData
{
    public static class Extensions
    {
        public static void InitBaseEntity<T>(this EntityTypeBuilder<T> item) where T : BaseEntity
        {
            item.HasKey(o => o.Id);
            item.Property(o => o.Created).IsRequired(false).ValueGeneratedOnAdd();
            item.Property(o => o.Modified).IsRequired(false).ValueGeneratedOnAdd();
        }

        public static void InitBaseShopperChildEntity<T>(
            this EntityTypeBuilder<T> item,
            Expression<Func<ShopperEntity, IEnumerable<T>>> navigationExpression)
            where T : BaseShopperChildEntity
        {
            item.InitBaseEntity();
            item.Property(o => o.ShopperId).IsRequired();
            item.HasOne(o => o.Shopper).WithMany(navigationExpression);
        }

        public static void InitBaseShopperFriendEntity<T>(
            this EntityTypeBuilder<T> item,
            Expression<Func<ShopperEntity, IEnumerable<T>>> navigationExpression)
            where T : BaseShopperFriendEntity
        {
            item.InitBaseShopperChildEntity(navigationExpression);
            item.Property(o => o.Email).IsRequired();
            item.Property(o => o.ProfileImageUrl).IsRequired(false).ValueGeneratedOnAdd();
            item.Property(o => o.Name).IsRequired();
        }
    }
}
