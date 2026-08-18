using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Interfaces;

namespace UrlShortener.API.Controllers
{
    [Route("")]
    [ApiController]
    public class RedirectController : ControllerBase
    {
        public readonly IRedirectService _redirectService;

        public RedirectController(IRedirectService redirectService)
        {
            _redirectService = redirectService;
        }
        [HttpGet("{code}")]
        public async Task<IActionResult> RedirectToOriginalUrl(string shortCode)
        {
            try
            {
                var originalUrl = await _redirectService.Redirect(shortCode);
                if (string.IsNullOrEmpty(originalUrl))
                {
                    return NotFound("Short URL not found.");
                }
                return Redirect(originalUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
