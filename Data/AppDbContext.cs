using FridgeProject.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
//مهم تنتبه هون 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FridgeProject.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Category> categories { get; set; }
        public DbSet<Item> items { get; set; }
        public DbSet<UserItem> userItem { get; set; }
        
        public DbSet<Recipe> Recipes { get; set; } 
        public DbSet<RecipeItem> RecipeItems { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Meal" },
                 new Category { Id = 2, Name = "vegetables" },
                  new Category { Id = 3, Name = "Cooking" }
                );
        }
    }
}
