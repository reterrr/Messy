using Messy.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messy.Contexts.Configurations;

public abstract class CreatableConfiguration<TCreateable> : IEntityTypeConfiguration<TCreateable>
    where TCreateable : class, ITimeStampable
{
    public void Configure(EntityTypeBuilder<TCreateable> builder)
    {
        builder.Property(cr => cr.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        builder.Property(cr => cr.UpdatedAt)
            .HasDefaultValueSql(null)
            .ValueGeneratedOnAdd();
    }
}