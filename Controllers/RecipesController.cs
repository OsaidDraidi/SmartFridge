using FridgeProject.Data;
using FridgeProject.Data.Models;
using FridgeProject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FridgeProject.Controllers
{
    [Route("api/recipe")]
    [ApiController]
    [Authorize]
    public class RecipesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RecipesController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipe()
        {
            return Ok(GetAllRecipes());
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipe(int id)
        {
            //var recipe = await _context.Recipes.FindAsync(id);
            var recipe = GetRecipeById(id);

            if (recipe == null)
            {
                return NotFound();
            }

            return Ok(recipe);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int id, dtoRecipe dtoRecipe)
        {
            var recipe = await _context.Recipes.FindAsync(id);

            if (_isNotSameCreator(recipe.CreatorId)) { return Unauthorized(); }

            if (recipe == null || dtoRecipe == null)
            {
                return BadRequest();
            }


            recipe.Name = dtoRecipe.Name;
            recipe.Description = dtoRecipe.Description;
            var log = new AuditLog
            {
                UserId = _GetUserIdFromToken(),
                Action = "Update",
                EntityName = "Recipes",
                EntityId = recipe.Id,
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(log);

            var existingRecipeItems=await _context.RecipeItems
                //.AsNoTracking()
                .Where(ri=>ri.RecipeId == id)
                //.Select(ri => new
                //{
                //    ri.Id,
                //    ri.ItemId,
                //    ri.Quantity
                //})
                .ToListAsync();


            List<dtoCreateRecipeItem> items = dtoRecipe.items;

            foreach (var item in items)
            {
                //here return recipeItem
                var existingItem = existingRecipeItems.FirstOrDefault(ri => ri.ItemId == item.ItemId);
                if (existingItem != null)
                {
                    //here return recipeItem
                    var recipeItem = _context.RecipeItems.FirstOrDefault(ri => ri.Id == existingItem.Id);


                    if (recipeItem != null)
                    {
                        recipeItem.Quantity = item.RequirdQuantity;  // تحديث الكمية
                        _context.RecipeItems.Update(recipeItem);  // تحديث العنصر في قاعدة البيانات
                        var auditlog = new AuditLog
                        {
                            UserId = _GetUserIdFromToken(),
                            Action = "Update",
                            EntityName = "RecipeItems",
                            EntityId = recipeItem.Id,
                            Timestamp = DateTime.UtcNow
                        };
                        _context.AuditLogs.Add(auditlog);
                    }
                }
                else
                {
                    // add new item
                    var newRecipeItem = new RecipeItem
                    {
                        RecipeId = id,
                        ItemId = item.ItemId,
                        Quantity = item.RequirdQuantity
                    };
                    _context.RecipeItems.Add(newRecipeItem);
                    var auditlog = new AuditLog
                    {
                        UserId = _GetUserIdFromToken(),
                        Action = "Create",
                        EntityName = "RecipeItems",
                        EntityId = newRecipeItem.Id,
                        Timestamp = DateTime.UtcNow
                    };
                    _context.AuditLogs.Add(auditlog);
                }
            }
            // remove non-existent items 
            var newItemIds = dtoRecipe.items.Select(i => i.ItemId).ToList();

            var itemsToRemove = existingRecipeItems
                .Where(ri => !newItemIds.Contains(ri.ItemId))
                .ToList();

            _context.RecipeItems.RemoveRange(itemsToRemove);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(recipe);
        }


        [HttpPost]
        public async Task<ActionResult<Recipe>> PostRecipe([FromBody] dtoRecipe dtoRecipe)
        {
            var creatorId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if(creatorId == null) { return Unauthorized(); }

            if (dtoRecipe == null) return BadRequest();

            Recipe recipe = new Recipe
            {
                Name = dtoRecipe.Name,
                Description = dtoRecipe.Description,
                CreatorId = creatorId,
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            var log = new AuditLog
            {
                UserId = _GetUserIdFromToken(),
                Action = "Create",
                EntityName = "Recipes",
                EntityId = recipe.Id,
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(log);

            await _context.SaveChangesAsync();
            var maxId = _context.Recipes.Any() ? _context.Recipes.Max(x => x.Id) : 0;
            foreach (var item in dtoRecipe.items)
            {

                RecipeItem recipeItem = new RecipeItem
                {

                    Quantity = item.RequirdQuantity,
                    ItemId = item.ItemId,
                    RecipeId = maxId
                };
                var auditlog = new AuditLog
                {
                    UserId = _GetUserIdFromToken(),
                    Action = "Create",
                    EntityName = "RecipeItems",
                    EntityId = recipeItem.Id,
                    Timestamp = DateTime.UtcNow
                };
                _context.AuditLogs.Add(auditlog);
                _context.RecipeItems.Add(recipeItem);
            }

            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, dtoRecipe);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (_isNotSameCreator(recipe.CreatorId)) { return Unauthorized(); }
            if (recipe == null)
            {
                return NotFound();
            }
            var log = new AuditLog
            {
                UserId = _GetUserIdFromToken(),
                Action = "Delete",
                EntityName = "Recipes",
                EntityId = recipe.Id,
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(log);
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpGet("User/Recipes/avaliable")]
        public async Task<IActionResult> AvailableRecipes()
        {
            //
            var useritems = await _context.userItem.Where(ui => EF.Functions.DateDiffDay(ui.CreateDate, DateOnly.FromDateTime(DateTime.Now)) < ui.Item.ExpiryDays).ToListAsync();
            Dictionary<int, int> itemsQuantity = new Dictionary<int, int>();
            foreach (var item in useritems)
            {

                if (itemsQuantity.ContainsKey(item.ItemId))
                {
                    itemsQuantity[item.ItemId] += item.Quantity;
                }
                else
                {
                    itemsQuantity.Add(item.ItemId, item.Quantity);
                }

            }
            var recipeItems = await _context.RecipeItems.ToListAsync();
            List<dtoGetRecipe> avalRecipes = [];

            foreach (var recipeitem in recipeItems)
            {

                if (itemsQuantity.ContainsKey(recipeitem.ItemId) && recipeitem.Quantity < itemsQuantity[recipeitem.ItemId])
                {
                    var recipe = GetRecipeById(recipeitem.RecipeId);
                    if (recipe != null)
                    {
                        dtoGetRecipe dd = recipe.FirstOrDefault();
                        avalRecipes.Add(dd);

                    }
                }
            }

            return Ok(avalRecipes);
        }
        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }

        private List<dtoGetRecipe> GetAllRecipes()
        {
            var recipes = _context.RecipeItems
                .Include(e => e.Item).ThenInclude(c => c.Category)
                .GroupBy(i => i.Recipe.Id)
                .Select(ri => new dtoGetRecipe
                {
                    Name = ri.Select(s => s.Recipe.Name).First(),
                    Description = ri.Select(s => s.Recipe.Description).First(),
                    items = ri.Select(i => new dtoGetRecipeItem
                    {

                        Id = i.Id,
                        Name = i.Item.Name,
                        RequirdQuantity = i.Quantity,
                        CategoryName = i.Item.Category.Name,

                        //Id = i.Item.Id,
                        //Name = i.Item.Name,
                        //CategoryId = i.Item.CategoryId,
                        //ExpiryDays = i.Item.ExpiryDays,

                    })
                        .GroupBy(i => i.Name)  // تجميع العناصر حسب ItemId
                        .Select(g => g.First())  // جلب أول عنصر من كل مجموعة
                        .ToList()

                })


                .ToList();
            return recipes;
        }
        private List<dtoGetRecipe> GetRecipeById(int recipeId)
        {

            var recipes = _context.RecipeItems
                .GroupBy(i => i.Recipe.Id)
                .Where(r => r.Key == recipeId)
                .Select(ri => new dtoGetRecipe
                {
                    Name = ri.Select(s => s.Recipe.Name).First(),
                    Description = ri.Select(s => s.Recipe.Description).First(),

                    items = ri.Select(i => new dtoGetRecipeItem
                    {

                        Id = i.Id,
                        Name = i.Item.Name,
                        RequirdQuantity = i.Quantity,
                        CategoryName = i.Item.Category.Name,
                        //Id = i.Item.Id,
                        //Name = i.Item.Name,
                        //CategoryId = i.Item.CategoryId,
                        //ExpiryDays = i.Item.ExpiryDays,

                    })
                        .GroupBy(i => i.Name)  // تجميع العناصر حسب ItemId
                        .Select(g => g.First())  // جلب أول عنصر من كل مجموعة
                        .ToList()

                })


                .ToList();
            return recipes;
        }
        private bool _existItem(string name)
        {
            return _context.items.Any(i => i.Name == name);
        }

        private dtoRecipeItem _putRecipeItem(int recipeItemId, dtoCreateRecipeItem dtoCreateRecipeItem)
        {
            var ri = _context.RecipeItems
                .Where(r => r.ItemId == dtoCreateRecipeItem.ItemId && r.RecipeId == recipeItemId)
                .Select(ri=> new dtoRecipeItem
                {
                    RecipeId = ri.RecipeId,
                    ItemId = dtoCreateRecipeItem.ItemId,
                    RequirdQuantity= dtoCreateRecipeItem.RequirdQuantity,
                })
                .FirstOrDefault();
            
            return ri;
        }
        private bool _isNotSameCreator(string creator)
        {
            var userId = _GetUserIdFromToken();
            var userEmail = _GetUserEmailFromToken();

            return creator != userId || userId == null || userEmail != "admin@example.com";

        }
        private string _GetUserIdFromToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            return userId;

        }
        private string _GetUserEmailFromToken()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;


            return userEmail;

        }
    }
}
