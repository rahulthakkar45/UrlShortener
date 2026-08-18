using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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

        public AuthService(IUserRepository userRepository,  ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmail(loginDto.Email);

            if (user is null)
            {
                throw new UnauthorizedAccessException("Invalid Credentials");
            }
            var isPasswordValid = PasswordHasher.VerifyPassword(user, user.PasswordHash, loginDto.Password);
            if (!isPasswordValid) 
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            var token = _tokenService.GenerateToken(user);
            return new AuthResponseDto { Token = token };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userRepository.GetByEmail(registerDto.Email);

            if(existingUser != null)
            {
                throw new InvalidOperationException("User already exists");
            }
            var user = new User
            {
                Email = registerDto.Email
            };
            user.PasswordHash = PasswordHasher.Hash(user, registerDto.Password);
            await _userRepository.AddUser(user);
            var token = _tokenService.GenerateToken(user);
            return new AuthResponseDto { Token = token };
        }
    }
}
