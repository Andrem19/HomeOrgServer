using HomeOrganizer.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace HomeOrganizer.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(DataContext context, UserManager<User> userManager, ILoggerFactory loggerFactory)
        {
            try
            {
                if (!userManager.Users.Any())
                {
                    var user = new User
                    {
                        Id = "e3b1803b-c3c9-4e88-9f57-6b3bf5c96a18",
                        UserName = "bob",
                        Email = "bob@test.com",
                        EmailConfirmed = true,
                        InviteCode = "YT65FCVBPO009GFQA3WSF65G"
                    };

                    await userManager.CreateAsync(user, "Pa$$w0rd");

                    var admin = new User
                    {
                        Id = "9f92f390-bd55-460d-bc3d-4128c463065e",
                        UserName = "admin",
                        Email = "admin@test.com",
                        InviteCode = "YT88FCVBPO009GFQA3WSF55G",
                        EmailConfirmed = true,
                    };
                    await userManager.CreateAsync(admin, "Pa$$w0rd");

                    var moderator = new User
                    {
                        Id = "7c2e0af3-d178-4ed2-a634-de98abce7313",
                        UserName = "moderator",
                        Email = "moderator@test.com",
                        InviteCode = "YT65FCVBPO449GFQA3WSF65G",
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(moderator, "Pa$$w0rd");

                    var delivery = new User
                    {
                        Id = "e3b1803a-c3c9-4e44-9f57-6b3bt6i96a18",
                        UserName = "delivery",
                        InviteCode = "YT65FCVBPO009GFQA3WSF23G",
                        Email = "delivery@test.com",
                        EmailConfirmed = true,
                    };

                    await userManager.CreateAsync(delivery, "Pa$$w0rd");
                };
                if (!context.Groups.Any())
                {
                    var groupData =
                        File.ReadAllText("../HomeOrganizer/Data/SeedData/groups.json");
                    var groups = JsonSerializer.Deserialize<List<Group>>(groupData);


                    foreach (var item in groups)
                    {
                        context.Groups.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<StoreContextSeed>();
                logger.LogError(ex.Message);
            }
        }
    }
}
