using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data;

public static class AdminSeeder
{
    public static async Task SeedAdminAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        var passwordHasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher<ApplicationUser>>();

        var email = configuration["Admin:Email"];
        var fullName = configuration["Admin:FullName"];
        var password = configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Admin configuration is missing. Please configure Admin details using User Secrets.");
        }

        email = email.Trim().ToLowerInvariant();

        var existingAdmin = await context.Set<ApplicationUser>()
            .FirstOrDefaultAsync(user => user.Email == email);

        if (existingAdmin != null)
        {
            if (existingAdmin.Role != "Admin")
            {
                throw new InvalidOperationException(
                    "The configured Admin email already belongs to a non-Admin user.");
            }

            return;
        }

        var admin = new ApplicationUser
        {
            FullName = fullName.Trim(),
            Email = email,
            Role = "Admin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        admin.PasswordHash = passwordHasher.HashPassword(
            admin,
            password);

        context.Set<ApplicationUser>().Add(admin);

        await context.SaveChangesAsync();
    }
}