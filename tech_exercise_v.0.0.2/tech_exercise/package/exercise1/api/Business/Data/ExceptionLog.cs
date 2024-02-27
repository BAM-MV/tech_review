using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data;

[Table("ExceptionLog")]
public class ExceptionLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }

    public string Level { get; set; } = string.Empty;
    public string Exception { get; set; } = string.Empty;
    public string RenderedMessage { get; set; } = string.Empty;
    public string Properties { get; set; } = string.Empty;

    public class ExceptionLogConfiguration : IEntityTypeConfiguration<ExceptionLog>
    {
        public void Configure(EntityTypeBuilder<ExceptionLog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
