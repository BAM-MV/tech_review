using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data;

public class SuccessLog
{
    public int Id { get; set; }
    //public int? PersonId { get; set; }
    public string Message { get; set; } = string.Empty;

    public DateTime TimeStamp { get; set; }
    //public virtual Person Person { get; set; }

    public class AstronautDutyConfiguration : IEntityTypeConfiguration<SuccessLog>
    {
        public void Configure(EntityTypeBuilder<SuccessLog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
