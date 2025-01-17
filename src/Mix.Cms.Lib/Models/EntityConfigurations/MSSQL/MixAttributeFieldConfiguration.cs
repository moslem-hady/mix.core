﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mix.Cms.Lib.Models.Cms;

namespace Mix.Cms.Lib.Models.EntityConfigurations.MSSQL
{
    public class MixAttributeFieldConfiguration : IEntityTypeConfiguration<MixAttributeField>
    {
        public void Configure(EntityTypeBuilder<MixAttributeField> entity)
        {
            entity.ToTable("mix_attribute_field");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.AttributeSetName).HasMaxLength(250);

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");

            entity.Property(e => e.DefaultValue).HasColumnType("ntext");

            entity.Property(e => e.LastModified).HasColumnType("datetime");

            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
                
            entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Options).HasColumnType("ntext");

            entity.Property(e => e.Regex).HasMaxLength(4000);
        }
    }
}
