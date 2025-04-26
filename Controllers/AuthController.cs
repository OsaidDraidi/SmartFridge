using FridgeProject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FridgeProject.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase

    {
        //      *****  لماذا نستخدم UserManager<AppUser> بدلاً من DbContext مباشرةً؟ ******
        //1️ الأمان: UserManager يتعامل مع تشفير كلمة المرور تلقائيًا.
        //2️ الإدارة السهلة: يوفر وظائف جاهزة دون الحاجة لكتابة استعلامات معقدة.
        //3️ الدعم المدمج لـ Identity: يعمل مع ASP.NET Core Identity مما يجعله متكاملًا مع النظام.

        //هو كائن إدارة المستخدمين في ASP.NET Core Identity
        // يوفر مجموعة من الوظائف الجاهزة للتعامل مع المستخدمين
        private readonly UserManager<IdentityUser> _userManager;

        private readonly IConfiguration _configuration;
        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Register([FromBody] dtoRegister dtoRegister)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new IdentityUser
            {
                UserName = dtoRegister.Username,
                Email = dtoRegister.Email,
            };
            // ننشئ مستخدم جديد مع باسورد مشفر 
            IdentityResult result = await _userManager.CreateAsync(user, dtoRegister.Password);

            // اوبجيكت يمثل نتيجة العملية
            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully!" });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] dtoLogin dtoLogin)
        {

            var user = await _userManager.FindByEmailAsync(dtoLogin.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dtoLogin.Password))
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
            var token = GenerateJwtToken(user);
            return Ok(new { token });

        }

        private string GenerateJwtToken(IdentityUser user)
        {
            // هنا نقوم بجلب المفتاح السري (Jwt:Key) من appsettings.json وتحويله إلى مفتاح مشفر يستخدمه JWT للتوقيع.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            //هنا نقول لـ JWT أن هذا المفتاح سيُستخدم في التوقيع باستخدام HMAC SHA256.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            //Claims هي البيانات التي يتم تضمينها داخل التوكن،
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                //Jti (JWT ID): معرف فريد لكل توكن.
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                //new Claim(ClaimTypes.Role,"Admin")
            };
            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

            }


            var token = new JwtSecurityToken(
                //الخادم الذي أصدر التوكن.
                issuer: _configuration["Jwt:Issuer"],
                //الجهة المستهدفة.
                audience: _configuration["Jwt:Audience"],
                //بيانات المستخدم.
                claims: claims,

                expires: DateTime.Now.AddHours(48),
                //التوقيع: باستخدام المفتاح السري.
                signingCredentials: creds
                );

            //هنا يتم تحويل التوكن إلى نص
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
