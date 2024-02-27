using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data;

[Table("SuccessLog")]
public class SuccessLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }

    public string Level { get; set; } = string.Empty;
    public string Exception { get; set; } = string.Empty;
    public string RenderedMessage { get; set; } = string.Empty;
    public string Properties { get; set; } = string.Empty;

    public class SuccessLogConfiguration : IEntityTypeConfiguration<SuccessLog>
    {
        public void Configure(EntityTypeBuilder<SuccessLog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
