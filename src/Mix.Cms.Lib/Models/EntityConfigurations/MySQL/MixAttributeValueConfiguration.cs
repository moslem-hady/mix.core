﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mix.Cms.Lib.Models.Cms;

using Mix.Cms.Lib.Enums;
using Mix.Cms.Lib.Constants;
namespace Mix.Cms.Lib.Models.EntityConfigurations.MySQL
{
    public class MixAttributeValueConfiguration : IEntityTypeConfiguration<MixAttributeSetValue>
    {
        public void Configure(EntityTypeBuilder<MixAttributeSetValue> builder)
        {
            builder
           .Property(e => e.DataType)
           .HasConversion(new EnumToStringConverter<MixDataType>())
           .HasColumnType("varchar(50)");
        }
    }
}
