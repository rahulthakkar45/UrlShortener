using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.DTOs.Url;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entitites;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories
{
    public class ShortUrlRepository : IShortUrlRepository
    {
        private readonly UrlShortenerDbContext _context;

        public ShortUrlRepository(UrlShortenerDbContext context)
        {
            _context = context;
        }

        public async Task<ShortUrl> GetByIdAsync(int id, int userId)
        {
            return await _context.ShortUrls.AsNoTracking().Include(x => x.Clicks).FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        }
        public async Task<ShortUrl> GetByShortCodeAsync(string shortCode)
        {
            return await _context.ShortUrls.FirstOrDefaultAsync(x => x.ShortCode == shortCode);
        }
        public async Task<bool> ShortCodeExistsAsync(string shortCode)
        {
            return await _context.ShortUrls.AnyAsync(x => x.ShortCode == shortCode);
        }
        public async Task AddAsync(ShortUrl shortUrl)
        {
            await _context.ShortUrls.AddAsync(shortUrl);
            await _context.SaveChangesAsync();
        }
        public async Task<ShortUrl> GetByShortCodeForRedirectionAsync(string shortCode)
        {
            return await _context.ShortUrls.FirstOrDefaultAsync(x => x.ShortCode == shortCode);
        }
        public async Task<(List<ShortUrl> shortUrls, int totalCount)> GetAllAsync(int userId, UrlListDto dto)
        {
            var query = _context.ShortUrls.Where(x => x.UserId == userId);
            if (!string.IsNullOrWhiteSpace(dto.Search))
            {
                var search = dto.Search.Trim();
                query = query.Where(x => x.OriginalUrl.Contains(search) || x.ShortCode.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                if (dto.Status == "active")
                {
                    query = query.Where(x => x.IsActive);
                }
                else if (dto.Status == "expired")
                {
                    query = query.Where(x => x.ExpirationDate < DateTime.UtcNow);
                }
                else if (dto.Status == "inactive")
                {
                    query = query.Where(x => !x.IsActive);
                }
            }

            var totalCount = await query.CountAsync();
            query = dto.SortBy switch
            {
                "originalUrl" => query.OrderBy(x => x.OriginalUrl),
                "shortCode" => query.OrderBy(x => x.ShortCode),
                "createdAt" => query.OrderBy(x => x.CreatedAt),
                _ => query.OrderByDescending(x => x.CreatedAt)
            };

            var shortUrls = await query
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToListAsync();
            return (shortUrls, totalCount);
        }
        public async Task<UrlStatisticsDto> GetUrlStatisticsAsync(int Id, int userId)
        {
            var last7Days = DateTime.UtcNow.AddDays(-7);
            var shortUrl = await _context.ShortUrls.AsNoTracking().Where(x => x.Id == Id && x.UserId == userId)
                .Select(x => new UrlStatisticsDto
                {
                    TotalClicks = x.Clicks.Count,
                    ClicksToday = x.Clicks.Count(c => c.ClickedAt.Date >= DateTime.UtcNow.Date && c.ClickedAt <= DateTime.UtcNow),
                    clicksLast7Days = x.Clicks.Count(c => c.ClickedAt >= last7Days && c.ClickedAt <= DateTime.UtcNow),
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();
            return shortUrl;
        }
        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var shortUrl = await _context.ShortUrls.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (shortUrl is null)
            {
                return false;
            }
            shortUrl.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
