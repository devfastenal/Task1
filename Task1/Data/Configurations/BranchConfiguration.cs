using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Task1.Data.Configurations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasData(
                    new Branch
                    {
                        Id = 1,
                        BU_Codes = "AB100",
                        Status = "Open",
                        Opened_dt = new DateOnly(2008, 09, 15),
                        Address = "19020 111th Ave",
                        CityId = 1,
                        Phone = "780-801-5006",
                        Business_Hours = "Monday - Friday 7:30am - 5:00pm",
                        Latitude = 54,
                        Longitude = -114,
                    },
                    new Branch
                    {
                        Id = 2,
                        BU_Codes = "ABACH",
                        Status = "Open",
                        Opened_dt = new DateOnly(2008, 06, 26),
                        Address = "26229 Twp 531 A Unit 115",
                        CityId = 2,
                        Phone = "(780)-960-4120",
                        Business_Hours = "Monday - Friday 7:30am - 5:00pm",
                        Latitude = 54,
                        Longitude = -114,
                    }
                );
        }
    }
}
