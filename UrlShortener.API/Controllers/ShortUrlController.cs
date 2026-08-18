using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.DTOs.Url;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.API.Controllers
{
    [Route("api/urls")]
    [ApiController]
    [Authorize]
    public class ShortUrlController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;

        public ShortUrlController(IShortUrlService shortUrlService)
        {
            _shortUrlService = shortUrlService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<ShortUrlResponseDto>> CreateShortUrl([FromBody] CreateShortUrlDto dto)
        {
            var userId = GetUserId();
            var response = await _shortUrlService.Create(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ShortUrlResponseDto>> GetById(int id)
        {
            var userId = GetUserId();
            var response = await _shortUrlService.GetByIdAsync(id, userId);
            if (response is null)
            {
                throw new KeyNotFoundException("Short URL not found.");
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<ShortUrlResponseDto>>> GetAll([FromQuery] UrlListDto itemDto)
        {
            var userId = GetUserId();          
            var response = await _shortUrlService.GetAllAsync(userId, itemDto);
            if(response is null)
            {
                throw new KeyNotFoundException("No short URLs found.");
            }
            return Ok(response);
        }

        [HttpGet("{id:int}/statistics")]
        public async Task<ActionResult<UrlStatisticsDto>> GetUrlStatistics(int id)
        {
            var userId = GetUserId();
            var response = await _shortUrlService.GetUrlStatisticsAsync(id, userId);
            if (response is null)
            {
                throw new KeyNotFoundException("Short URL not found.");
            }
            return Ok(response);
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var result = await _shortUrlService.DeleteAsync(id, userId);
            if (!result)
            {
                throw new KeyNotFoundException("Short URL not found or you do not have permission to delete it.");
            }
            return NoContent();
        }

        private int GetUserId()
        {
            var claims = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            if (!int.TryParse(claims, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID.");
            }
            return userId;
        }
    }
}
