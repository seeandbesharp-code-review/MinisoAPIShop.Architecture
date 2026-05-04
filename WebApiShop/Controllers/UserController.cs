using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using WebApiShop.Attributes;


namespace WebAPIShop.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase, IUserController
    {
        private readonly IUserService _userService;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, IPasswordService passwordService, ILogger<UserController> logger)
        {
            _userService = userService;
            _passwordService = passwordService;
            _logger = logger;
        }

        // GET: api/<UsersController>
        [HttpGet]
        async public Task<IEnumerable<string>> Get()
        {
            return new string[] { "" };
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadDTO>> Get(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // POST api/<UsersController>
        [HttpPost("register")]
        public async Task<ActionResult<UserReadDTO>> Post([FromBody] UserRegisterDTO userRegisterDto)
        {
            try
            {
                var newUser = await _userService.AddUser(userRegisterDto);
                if (newUser == null)
                    return BadRequest("שגיאה ביצירת המשתמש.");
                var token = _userService.GenerateToken(newUser);
                AppendSecretCookie(token);
                _logger.LogInformation("New user registration: Name: {FullName}, Email: {Email}",
                    userRegisterDto.FirstName + " " + userRegisterDto.LastName, userRegisterDto.Email);
                return CreatedAtAction(nameof(Get), new { id = newUser.UserId }, newUser);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserReadDTO>> Login([FromBody] UserLoginDTO loginDto)
        {
            var user = await _userService.LoginUser(loginDto);
            if (user != null) {
                var token = _userService.GenerateToken(user);
                AppendSecretCookie(token);
                _logger.LogInformation("User logged in successfully: Name: {FullName}", $"{user.FirstName} {user.LastName}");
                return Ok(user);
            }
            return Unauthorized();
        }

        private void AppendSecretCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddDays(1)
            };
            Response.Cookies.Append("X-Access-Token", token, cookieOptions);
        }

        // PUT api/<UsersController>/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserUpdateDTO userUpdateDto)
        {
            bool isUpdated = await _userService.UpdateUser(id, userUpdateDto);
            if (!isUpdated)
                return BadRequest("Update failed. Please check password strength.");
            return NoContent();
        }

        // DELETE api/<UsersController>/5
        [AuthorizeAdmin]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
