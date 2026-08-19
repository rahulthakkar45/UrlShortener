using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.DTOs.Auth;
using UrlShortener.Application.Helpers;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entitites;

namespace UrlShortener.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository,  ITokenService tokenService, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmail(loginDto.Email);

            if (user is null)
            {
                _logger.LogWarning("Authentication failed for email: {Email}. User not found.", loginDto.Email);
                throw new UnauthorizedAccessException("Invalid Credentials");
            }
            var isPasswordValid = PasswordHasher.VerifyPassword(user, user.PasswordHash, loginDto.Password);
            if (!isPasswordValid) 
            {
                _logger.LogWarning("Authentication failed for email: {Email}. Invalid password.", loginDto.Email);
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            var token = _tokenService.GenerateToken(user);
            _logger.LogInformation("User {Email} authenticated successfully.", loginDto.Email);
            return new AuthResponseDto { Token = token };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userRepository.GetByEmail(registerDto.Email);

            if(existingUser != null)
            {
                _logger.LogWarning("Registration failed for email: {Email}. User already exists.", registerDto.Email);
                throw new InvalidOperationException("User already exists");
            }
            var user = new User
            {
                Email = registerDto.Email
            };
            user.PasswordHash = PasswordHasher.Hash(user, registerDto.Password);
            await _userRepository.AddUser(user);
            _logger.LogInformation("User {Email} registered successfully.", registerDto.Email);
            var token = _tokenService.GenerateToken(user);
            return new AuthResponseDto { Token = token };
        }
    }
}
