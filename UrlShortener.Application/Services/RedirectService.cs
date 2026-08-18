using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entitites;

namespace UrlShortener.Application.Services
{
    public class RedirectService : IRedirectService
    {
        private readonly IClickRepository _clickRepository;
        private readonly IShortUrlRepository _shortUrlRepository;

        public RedirectService(IClickRepository clickRepository, IShortUrlRepository shortUrlRepository)
        {
            _clickRepository = clickRepository;
            _shortUrlRepository = shortUrlRepository;
        }

        public async Task<string> Redirect(string shortCode)
        {
            var shortUrl = await _shortUrlRepository.GetByShortCodeForRedirectionAsync(shortCode);
            if (shortUrl is null)
            {
                throw new KeyNotFoundException("Short URL not found");
            }
            if(!shortUrl.IsActive)
            {
                throw new InvalidOperationException("Short URL is inactive");
            }
            if(shortUrl.ExpirationDate <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Short URL has expired");
            }
            var click = new Click
            {
                ShortUrlId = shortUrl.Id,
                ClickedAt = DateTime.UtcNow
            };
            await _clickRepository.Add(click);
            return shortUrl.OriginalUrl;
        }
    }
}
