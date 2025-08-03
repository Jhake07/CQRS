using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CQRS.Identity.Config
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            // builder.HasData(
            //    new IdentityUserRole<string>
            //    {
            //        RoleId = "cbc23a8e-f7bb-4445-baaf-1add431ffbbf",
            //        UserId = "AdminUser"
            //    },
            //    new IdentityUserRole<string>
            //    {
            //        RoleId = "cac20a6e-f7bb-4448-baaf-1add431ccbbf",
            //        UserId = "NormalUser"
            //    }
            //);
        }
    }
}
