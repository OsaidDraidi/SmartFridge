using FridgeProject.Data.Models.GeminiModels;
using FridgeProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FridgeProject.Controllers
{
    [Route("api/gemini")]
    [ApiController]
    public class GeminiController : ControllerBase
    {
        private readonly IGeminiService _geminiService;
        public GeminiController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }
        [HttpPost("ask")]
        public async Task<IActionResult> AskGemini([FromBody] PromptRequest request)
        {
            var response = await _geminiService.GetGeminiResponseAsync(request.Prompt);
            return Ok(new { response });
        }
    }
}
