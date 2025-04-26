using FridgeProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FridgeProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AuditLogController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        [HttpGet("logs")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAuditLogs()
        {
            var logs = await _context.AuditLogs
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
            return Ok(logs);
        }
    }
}
