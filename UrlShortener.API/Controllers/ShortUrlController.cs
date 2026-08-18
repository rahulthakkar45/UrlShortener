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
            try
            {
                var claims = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
                if (!int.TryParse(claims, out var userId))
                {
                    return Unauthorized("Invalid user ID.");
                }
                var response = await _shortUrlService.Create(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ShortUrlResponseDto>> GetById(int id)
        {
            var claims = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            if (!int.TryParse(claims, out var userId))
            {
                return Unauthorized("Invalid user ID.");
            }
            var response = await _shortUrlService.GetByIdAsync(id, userId);
            if (response is null)
            {
                return NotFound("Short URL not found.");
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<ShortUrlResponseDto>>> GetAll([FromQuery] UrlListDto itemDto)
        {
            var claims = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            if (!int.TryParse(claims, out var userId))
            {
                return Unauthorized("Invalid user ID.");
            }
            var response = await _shortUrlService.GetAllAsync(userId, itemDto);
            return Ok(response);
        }

        [HttpGet("{id:int}/statistics")]
        public async Task<ActionResult<UrlStatisticsDto>> GetUrlStatistics(int id)
        {
            var claims = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            if (!int.TryParse(claims, out var userId))
            {
                return Unauthorized("Invalid user ID.");
            }
            var response = await _shortUrlService.GetUrlStatisticsAsync(id, userId);
            if (response is null)
            {
                return NotFound("Short URL not found.");
            }
            return Ok(response);
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var claims = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            if (!int.TryParse(claims, out var userId))
            {
                return Unauthorized("Invalid user ID.");
            }
            var result = await _shortUrlService.DeleteAsync(id, userId);
            if (!result)
            {
                return NotFound("Short URL not found.");
            }
            return NoContent();
        }
    }
}
