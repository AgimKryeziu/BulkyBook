using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using BulkyBook.Utiltiy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._db = db;
        }

        public async Task InitializeAsync()
        {

            try
            {
                if ((await _db.Database.GetPendingMigrationsAsync()).Any())
                {
                    await _db.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {

            }

            if (!await _roleManager.RoleExistsAsync(SD.RoleCustomer))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.RoleCustomer));
                await _roleManager.CreateAsync(new IdentityRole(SD.RoleAdmin));
                await _roleManager.CreateAsync(new IdentityRole(SD.RoleEmployee));
            }

            ApplicationUser user = await _userManager.FindByEmailAsync("admin@bulkybook.com");
            if (user == null)
            {
                var result = await _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@bulkybook.com",
                    Email = "admin@bulkybook.com",
                    EmailConfirmed = true,
                    Name = "Bulky book",
                    PhoneNumber = "1112223333",
                    StreetAddress = "Bajram Kelmendi",
                    State = "PR",
                    PostalCode = "10000",
                    City = "Pristina"
                }, "Admin123*");

                if (result.Succeeded)
                {
                    user = await _userManager.FindByEmailAsync("admin@dotnetmastery.com");
                    await _userManager.AddToRoleAsync(user, SD.RoleAdmin);
                }
            }
        }
    }
}
