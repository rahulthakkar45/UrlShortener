using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs.Url;

namespace UrlShortener.Application.Interfaces
{
    public interface IShortUrlService
    {
        Task<ShortUrlResponseDto> Create(CreateShortUrlDto dto, int userId);

        Task<ShortUrlResponseDto> GetByIdAsync(int id, int userId);

        Task<PagedResultDto<UrlListItemDto>> GetAllAsync(int userId, UrlListDto dto);
        Task<UrlStatisticsDto> GetUrlStatisticsAsync(int Id, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
