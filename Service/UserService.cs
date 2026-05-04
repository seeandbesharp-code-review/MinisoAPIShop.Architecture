using AutoMapper;
using DTOs;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Zxcvbn;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IPasswordService passwordService, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _mapper = mapper;
            _configuration = configuration;
        }


        public string GenerateToken(UserReadDTO user)
        {
            string userRole = (user.Role?.ToLower() == "admin") ? "Admin" : "User";
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, userRole)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserReadDTO> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);
            return _mapper.Map<UserReadDTO>(user);
        }

        public async Task<UserReadDTO> AddUser(UserRegisterDTO userRegisterDto)
        {
            if (!_passwordService.IsPasswordStrong(userRegisterDto.Password))
            {
                throw new Exception("הסיסמה חלשה מדי. נסה לשלב אותיות, מספרים ותווים מיוחדים.");
            }

            if (await _userRepository.IsEmailExistsAsync(userRegisterDto.Email))
            {
                throw new Exception("כתובת האימייל כבר קיימת במערכת.");
            }

            User user = _mapper.Map<User>(userRegisterDto);
            User newUser = await _userRepository.AddUser(user);
            return _mapper.Map<UserReadDTO>(newUser);
        }

        public async Task<UserReadDTO> LoginUser(UserLoginDTO userLoginDto)
        {
            User userCredentials = _mapper.Map<User>(userLoginDto);
            User user = await _userRepository.LoginUser(userCredentials);
            return _mapper.Map<UserReadDTO>(user);
        }

        public async Task<bool> UpdateUser(int id, UserUpdateDTO userUpdateDto)
        {
            User user = _mapper.Map<User>(userUpdateDto);
            await _userRepository.UpdateUser(id, user);
            return true;
        }

    }
}
