using System;
using Microsoft.EntityFrameworkCore;
using PlatformService.Models;
namespace PlatformService.Data
{
    public static class PrepDb
    {

        public static void PrepData(IApplicationBuilder app, bool isProduction)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
            }
        }

        private static void SeedData(AppDbContext appDbContext, bool isProduction)
        {
            if (isProduction)
            {
                Console.WriteLine("Attempting to Apply Migrations..............");
                try{
                    appDbContext.Database.Migrate();
                }
                catch(Exception ex)
                {
                    System.Console.WriteLine("Couldnot run the migration due to Error!!!"+ex.Message);
                }
            }

            if (!appDbContext.Platforms.Any())
            {
                Console.WriteLine("****Seeding Data Started *****");
                appDbContext.Platforms.AddRange(

                       new Platform() { Name = "SQL Server", Publisher = "Microsoft", Cost = "Open Source" },
                       new Platform() { Name = "DotNet", Publisher = "Microsoft", Cost = "Open Source" },
                       new Platform() { Name = "Azure", Publisher = "Microsoft", Cost = "Cost Involved" },
                       new Platform() { Name = "Kubernetes", Publisher = "Cloud Native", Cost = "Open Source" }
                );
                appDbContext.SaveChanges();



            }
            else
            {
                Console.WriteLine("We have data there.. No seeding required");
            }
        }
    }
}
