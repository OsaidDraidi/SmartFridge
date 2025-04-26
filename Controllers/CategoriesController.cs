using FridgeProject.Data;
using FridgeProject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FridgeProject.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }
        private readonly AppDbContext _context;

        [HttpGet]
        //(Roles = "Admin")
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var category = await _context.categories
                .Select(p => new dtoCategory
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();
            return Ok(category);
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.categories
                .Where(c => c.Id == id)
                .Select(p => new dtoCategory
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }
    }
}
