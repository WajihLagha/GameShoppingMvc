using GameShoppingMvcUI.Constants;
using Microsoft.AspNetCore.Identity;

namespace GameShoppingMvcUI.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            var roleMgr = service.GetService<RoleManager<IdentityRole>>();
            var userMgr = service.GetService<UserManager<IdentityUser>>();
            //adding some roles to db
            await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));

            // create admin user if not exists
            var admin = new IdentityUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            };

            var userInDb = await userMgr.FindByNameAsync(admin.Email);
            if (userInDb is null)
            {
                var result = await userMgr.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
                }
            }
        }
    }
}
