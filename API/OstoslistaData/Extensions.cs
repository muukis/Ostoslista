using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
