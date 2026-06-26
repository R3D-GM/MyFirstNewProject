using Microsoft.AspNetCore.Identity;
using MyFirstNewProject.Models;

namespace MyFirstNewProject.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Create roles
        string[] roleNames = { "Admin", "Manager", "User" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create Admin User
        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin",
                CreatedAt = DateTime.Now
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Create Manager User
        var managerEmail = "manager@example.com";
        var managerUser = await userManager.FindByEmailAsync(managerEmail);

        if (managerUser == null)
        {
            managerUser = new ApplicationUser
            {
                UserName = managerEmail,
                Email = managerEmail,
                FullName = "Department Manager",
                CreatedAt = DateTime.Now
            };

            var result = await userManager.CreateAsync(managerUser, "Manager@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(managerUser, "Manager");
            }
        }

        // Create Regular User
        var userEmail = "user@example.com";
        var regularUser = await userManager.FindByEmailAsync(userEmail);

        if (regularUser == null)
        {
            regularUser = new ApplicationUser
            {
                UserName = userEmail,
                Email = userEmail,
                FullName = "Regular User",
                CreatedAt = DateTime.Now
            };

            var result = await userManager.CreateAsync(regularUser, "User@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(regularUser, "User");
            }
        }
    }
}