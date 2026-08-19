using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.DTOs.Url;
using UrlShortener.Application.Helpers;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entitites;

namespace UrlShortener.Application.Services
{
    public class ShortUrlService : IShortUrlService
    {
        private readonly IShortUrlRepository _shortUrlRepository;
        private readonly ILogger<ShortUrlService> _logger;

        public ShortUrlService(IShortUrlRepository shortUrlRepository, ILogger<ShortUrlService> logger)
        {
            _shortUrlRepository = shortUrlRepository;
            _logger = logger;
        }

        public async Task<ShortUrlResponseDto> Create(CreateShortUrlDto dto, int userId)
        {
            ValidateUrl(dto.Url);
            if (dto.ExpiresAt <= DateTime.UtcNow)
            {
                _logger.LogWarning("Attempted to create a short URL with an expiration date in the past for user {UserId}.", userId);
                throw new ArgumentException("Expiration can't be in the past.");
            }
            string shortCode;
            do
            {
                shortCode = ShortCodeGenerator.Generate();
            }
            while (await _shortUrlRepository.ShortCodeExistsAsync(shortCode));

            var shortUrl = new ShortUrl
            {
                OriginalUrl = dto.Url,
                ShortCode = shortCode,
                ExpirationDate = dto.ExpiresAt,
                IsActive = true,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _shortUrlRepository.AddAsync(shortUrl);
            _logger.LogInformation("Created new short URL with ID {ShortUrlId} for user {UserId}.", shortUrl.Id, userId);

            return new ShortUrlResponseDto
            {
                Id = shortUrl.Id,
                OriginalUrl = shortUrl.OriginalUrl,
                ShortCode = shortUrl.ShortCode,
                CreatedAt = shortUrl.CreatedAt,
                ExpiresAt = shortUrl.ExpirationDate,
                IsActive = shortUrl.IsActive,
                ClickCount = 0
            };
        }

        private static Uri ValidateUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("URL cannot be null or empty.");
            }
            if (!Uri.TryCreate(url, UriKind.Absolute, out var validatedUrl))
            {
                throw new ArgumentException("Invalid URL format.");
            }
            if (validatedUrl.Scheme != Uri.UriSchemeHttp && validatedUrl.Scheme != Uri.UriSchemeHttps)
            {
                throw new ArgumentException("Only HTTP & HTTPS URLs are supported.");
            }
            return validatedUrl;
        }

        public async Task<ShortUrlResponseDto> GetByIdAsync(int id, int userId)
        {
            var shortUrl = await _shortUrlRepository.GetByIdAsync(id, userId);
            if (shortUrl is null)
            {
                _logger.LogWarning("Short URL with ID {ShortUrlId} not found for user {UserId}.", id, userId);
                throw new KeyNotFoundException("Short URL not found.");
            }
            return new ShortUrlResponseDto
            {
                Id = shortUrl.Id,
                OriginalUrl = shortUrl.OriginalUrl,
                ShortCode = shortUrl.ShortCode,
                CreatedAt = shortUrl.CreatedAt,
                ExpiresAt = shortUrl.ExpirationDate,
                IsActive = shortUrl.IsActive,
                ClickCount = shortUrl.Clicks?.Count ?? 0
            };
        }

        public async Task<PagedResultDto<UrlListItemDto>> GetAllAsync(int userId, UrlListDto dto)
        {
            var (shortUrls, totalCount) = await _shortUrlRepository.GetAllAsync(userId, dto);
            var items = shortUrls.Select(s => new UrlListItemDto
            {
                Id = s.Id,
                OriginalUrl = s.OriginalUrl,
                ShortCode = s.ShortCode,
                CreatedAt = s.CreatedAt,
                ExpiresAt = s.ExpirationDate,
                IsActive = s.IsActive
            }).ToList();

            return new PagedResultDto<UrlListItemDto>(items, totalCount, dto.Page, dto.PageSize);
        }

        public async Task<UrlStatisticsDto> GetUrlStatisticsAsync(int id, int userId)
        {
            var shortUrl = await _shortUrlRepository.GetUrlStatisticsAsync(id, userId);
            if (shortUrl is null)
            {
                _logger.LogWarning("Statistics for short URL with ID {ShortUrlId} not found for user {UserId}.", id, userId);
                throw new KeyNotFoundException("Short URL not found.");
            }
            return shortUrl;
        }
        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var result = await _shortUrlRepository.DeleteAsync(id, userId);
            if (!result)
            {
                _logger.LogWarning("Attempted to delete short URL with ID {ShortUrlId} for user {UserId}, but it was not found or the user does not have permission.", id, userId);
                throw new KeyNotFoundException("Short URL not found.");
            }
            return result;
        }
    }
}
