using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.DTOs.Url
{
    public class CreateShortUrlDto
    {
        [Required]
        public string Url { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
