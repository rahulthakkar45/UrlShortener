using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.Url;
using UrlShortener.Domain.Entitites;

namespace UrlShortener.Application.Interfaces
{
    public interface IShortUrlRepository
    {
        Task<ShortUrl> GetByIdAsync(int id, int userId);
        Task<ShortUrl> GetByShortCodeAsync(string shortCode);
        Task<bool> ShortCodeExistsAsync(string shortCode);
        Task AddAsync(ShortUrl shortUrl);
        Task<ShortUrl> GetByShortCodeForRedirectionAsync(string shortCode);
        Task<(List<ShortUrl> shortUrls, int totalCount)> GetAllAsync(int userId, UrlListDto itemDto);
        Task<UrlStatisticsDto> GetUrlStatisticsAsync(int Id, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
