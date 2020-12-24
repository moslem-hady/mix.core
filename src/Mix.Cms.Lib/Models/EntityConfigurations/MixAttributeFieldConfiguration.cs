using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mix.Cms.Lib.Models.Cms;

namespace Mix.Cms.Lib.Models.EntityConfigurations
{
    public class MixAttributeFieldConfiguration : IEntityTypeConfiguration<MixDatabaseColumn>
    {
        public void Configure(EntityTypeBuilder<MixDatabaseColumn> builder)
        {
            builder
           .Property(e => e.DataType)
           .HasConversion(new EnumToStringConverter<MixEnums.MixDataType>())
           .HasColumnType("varchar(50)");
        }
    }
}
