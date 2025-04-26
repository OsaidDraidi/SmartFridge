using FridgeProject.Data;
using FridgeProject.Data.Models;
using FridgeProject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FridgeProject.Controllers
{
    [Route("api/user/items")]
    [ApiController]
    public class UserItemsController : ControllerBase
    {
        public UserItemsController(AppDbContext context)
        {
            _context = context;


        }
        private readonly AppDbContext _context;



        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.userItem
                .Include(p => p.Item)

                .Select(s => new dtoGetUserItem
                {
                    Id = s.Id,
                    ItemName = s.Item.Name,
                    CreateDate = s.CreateDate,
                    Quantity = s.Quantity,
                    CreatorEmail = s.CreatorEmail

                }).ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetById(int id)
        {

            var users = await _context.userItem
                .Where(p => p.Id == id)
                .Include(p => p.Item)
                .Select(s => new dtoGetUserItem
                {
                    Id = s.Id,
                    ItemName = s.Item.Name,
                    CreateDate = s.CreateDate,
                    Quantity = s.Quantity,
                    CreatorEmail = s.CreatorEmail

                }).ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUserItem([FromBody] dtoCreateUserItem dtoUserItem)
        {
            //User هو كائن من نوع ClaimsPrincipal يحتوي على جميع المعلومات الخاصة بالمستخدم المصادق عليه.
            var userId = _GetUserIdFromToken();
            var userEmail = _GetUserEmailFromToken();

            if (userId == null) { return Unauthorized(); }



            if (dtoUserItem == null) { return BadRequest("user item is null"); }
            bool itemExist = await _context.items.AnyAsync(a => a.Id == dtoUserItem.ItemId);
            var item = await _context.items.Where(w => w.Id == dtoUserItem.ItemId).SingleOrDefaultAsync();

            if (!itemExist || item == null) return NotFound("item not found");

            var userItem = new UserItem
            {
                CreateDate = DateOnly.FromDateTime(DateTime.Now),
                Quantity = dtoUserItem.Quantity,
                ItemId = dtoUserItem.ItemId,
                CreatorId = userId,
                CreatorEmail =userEmail

            };

            _context.userItem.Add(userItem);
            await _context.SaveChangesAsync();
            var log = new AuditLog
            {
                UserId = userId,
                Action = "Create",
                EntityName = "UserItem",
                EntityId = userItem.Id,
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = userItem.Id }, dtoUserItem);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserItem(int id, [FromBody] dtoCreateUserItem dtoUserItem)
        {

            if (dtoUserItem == null) { return BadRequest("useritem is null"); }
            //bool itemExist = await _context.items.AnyAsync(a => a.Id == dtoUserItem.ItemId);
            var item = await _context.items.Where(w => w.Id == dtoUserItem.ItemId).Select(p => new { p.Id }).SingleOrDefaultAsync();

            //if (!itemExist) return NotFound("item not found");
            if (item == null) return NotFound("item not found");

            var useritem = await _context.userItem.Where(w => w.Id == id).FirstOrDefaultAsync();

            if (_isNotSameCreator(useritem.CreatorId)) { return Unauthorized(); }

            if (useritem == null) { return NotFound("useritem is not found"); }
            useritem.Quantity = dtoUserItem.Quantity;
            useritem.ItemId = dtoUserItem.ItemId;

            var log = new AuditLog
            {
                UserId = _GetUserIdFromToken(),
                Action = "Update",
                EntityName = "UserItem",
                EntityId = useritem.Id,
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(log);

            await _context.SaveChangesAsync();
            return Ok(useritem);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUserItem(int id)
        {

            var userItem = await _context.userItem
                .Include(ui => ui.Item)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (userItem == null) return NotFound("useritem is not found");
            if (_isNotSameCreator(userItem.CreatorId)) { return Unauthorized(); }

            var log = new AuditLog
            {
                UserId = _GetUserIdFromToken(),
                Action = "delete",
                EntityName = "UserItem",
                EntityId = userItem.Id,
                Timestamp = DateTime.UtcNow
            };
            _context.AuditLogs.Add(log);
            _context.userItem.Remove(userItem);
            await _context.SaveChangesAsync();
            return Ok(new dtoGetUserItem
            {
                Id = userItem.Id,
                ItemName = userItem.Item.Name,
                CreateDate = userItem.CreateDate,
                Quantity = userItem.Quantity,
                CreatorEmail = userItem.CreatorEmail
            });

        }

        [HttpGet("users/items/expired")]
        [Authorize]
        public async Task<IActionResult> GetExpiredItems()
        {



            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            var useritems = await _context.userItem
                .Include(i => i.Item)
                .ThenInclude(i => i.Category)
                .Where(w => EF.Functions.DateDiffDay(w.CreateDate, today) > w.Item.ExpiryDays)
                .Select(s => new dtoExpiredItems
                {
                    Id = s.Id,
                    ItemName = s.Item.Name,
                    CategoryName = s.Item.Category.Name,
                    CreateDate = s.CreateDate,
                    Quantity = s.Quantity


                })
                .ToListAsync();

            //foreach (var item in useritems)
            //{
            //    var a = today.DayNumber-item.CreateDate.DayNumber;
            //}
            return Ok(useritems);

        }
        private bool _isNotSameCreator(string creator)
        {
            var userId = _GetUserIdFromToken();
            var userEmail = _GetUserEmailFromToken();

            return (creator != userId || userId == null || userEmail != "admin@example.com");

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
