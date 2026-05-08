using StudentManagementSystem.DTOs;
using StudentManagementSystem.Helpers;
using StudentManagementSystem.Models;
using StudentManagementSystem.Repositories.Interfaces;
using StudentManagementSystem.Services.Interfaces;

namespace StudentManagementSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            JwtHelper jwtHelper,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _jwtHelper      = jwtHelper;
            _logger         = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            _logger.LogInformation("Registering new user: {Username}", dto.Username);

            if (await _userRepository.UsernameExistsAsync(dto.Username))
                throw new InvalidOperationException($"Username '{dto.Username}' is already taken.");

            var user = new User
            {
                Username     = dto.Username.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role         = dto.Role.Trim(),
                CreatedDate  = DateTime.UtcNow
            };

            var created = await _userRepository.CreateAsync(user);
            _logger.LogInformation("User '{Username}' registered successfully.", created.Username);

            return BuildAuthResponse(created);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for user: {Username}", dto.Username);

            var user = await _userRepository.GetByUsernameAsync(dto.Username);
            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid credentials for user: {Username}", dto.Username);
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            _logger.LogInformation("User '{Username}' logged in successfully.", user.Username);
            return BuildAuthResponse(user);
        }

        private AuthResponseDto BuildAuthResponse(User user)
        {
            var (token, expiration) = _jwtHelper.GenerateToken(user);
            return new AuthResponseDto
            {
                Token      = token,
                Username   = user.Username,
                Role       = user.Role,
                Expiration = expiration
            };
        }
    }
}
