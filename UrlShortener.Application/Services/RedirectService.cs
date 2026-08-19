using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entitites;

namespace UrlShortener.Application.Services
{
    public class RedirectService : IRedirectService
    {
        private readonly IClickRepository _clickRepository;
        private readonly IShortUrlRepository _shortUrlRepository;
        private readonly ILogger<RedirectService> _logger;

        public RedirectService(IClickRepository clickRepository, IShortUrlRepository shortUrlRepository, ILogger<RedirectService> logger)
        {
            _clickRepository = clickRepository;
            _shortUrlRepository = shortUrlRepository;
            _logger = logger;
        }

        public async Task<string> Redirect(string shortCode)
        {
            var shortUrl = await _shortUrlRepository.GetByShortCodeForRedirectionAsync(shortCode);
            if (shortUrl is null)
            {
                _logger.LogWarning("Redirection failed for short code: {ShortCode}. Short URL not found.", shortCode);
                throw new KeyNotFoundException("Short URL not found");
            }
            if(!shortUrl.IsActive)
            {
                _logger.LogWarning("Redirection failed for short code: {ShortCode}. Short URL is inactive.", shortCode);
                throw new InvalidOperationException("Short URL is inactive");
            }
            if(shortUrl.ExpirationDate <= DateTime.UtcNow)
            {
                _logger.LogWarning("Redirection failed for short code: {ShortCode}. Short URL has expired.", shortCode);
                throw new InvalidOperationException("Short URL has expired");
            }
            var click = new Click
            {
                ShortUrlId = shortUrl.Id,
                ClickedAt = DateTime.UtcNow
            };
            await _clickRepository.Add(click);
            _logger.LogInformation("Redirection successful for short code: {ShortCode}. Redirecting to original URL: {OriginalUrl}", shortCode, shortUrl.OriginalUrl);
            return shortUrl.OriginalUrl;
        }
    }
}
