using FridgeProject.Data;
using FridgeProject.Data.Models;
using FridgeProject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

// api/categories/id/items ?

namespace FridgeProject.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        public ItemsController(AppDbContext context)
        {
            _context = context;
        }
        private readonly AppDbContext _context;



        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.items
                .Include(p => p.Category)
                .Select(x => new dtoGetItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    ExpiryDays = x.ExpiryDays,
                    CategoryName = x.Category.Name

                }).ToListAsync();


            return Ok(items);
        }


        [HttpGet("/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var items = await _context.items
                .Where(i => i.Id == id)
                .Include(p => p.Category)
                .Select(x => new dtoGetItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    ExpiryDays = x.ExpiryDays,
                    CategoryName = x.Category.Name

                }).ToListAsync();


            return Ok(items);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> CreateItem([FromBody] dtoCreateItem item_dto)

        {

            var userId = _GetUserIdFromToken();
            if (userId == null) { return Unauthorized(); }

            if (item_dto == null) return BadRequest("data is null");

            bool categoryExists = await _context.categories.AnyAsync(c => c.Id == item_dto.CategoryId);

            if (!categoryExists) return NotFound("Category not found");

            var item = new Item
            {

                Name = item_dto.Name,
                ExpiryDays = item_dto.ExpiryDays,
                CategoryId = item_dto.CategoryId,
                CreatorId = userId

            };
            var log = new AuditLog
            {
                UserId = userId,
                Action="Create",
                EntityName="Items",
                EntityId=item.Id,
                Timestamp = DateTime.UtcNow
            };
            _context.items.Add(item);
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditItem(int id, [FromBody] dtoCreateItem updatedItem)
        {



            var item = await _context.items.Where(w => w.Id == id).SingleOrDefaultAsync();
           
            if (_isNotSameCreator(item.CreatorId)) { return Unauthorized(); }
            if (item == null) return NotFound(item);
            if (updatedItem == null) return BadRequest("item is null");

            item.Name = updatedItem.Name;
            item.ExpiryDays = updatedItem.ExpiryDays;
            item.CategoryId = updatedItem.CategoryId;
           
            var log = new AuditLog
            {
                UserId = _GetUserIdFromToken(),
                Action = "Update",
                EntityName = "Items",
                EntityId = item.Id,
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
            return Ok(updatedItem);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.items.FirstOrDefaultAsync(w => w.Id == id);
            if (_isNotSameCreator(item.CreatorId)) { return Unauthorized(); }
            if (item == null) return NotFound("useritem is not found");
           
            var log = new AuditLog
            {
                UserId = _GetUserIdFromToken(),
                Action = "Delete",
                EntityName = "Items",
                EntityId = item.Id,
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(log);
            _context.items.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(new dtoGetItem{
                
                Name = item.Name,
                ExpiryDays = item.ExpiryDays,
                CategoryName = item.Category.Name
            });

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
