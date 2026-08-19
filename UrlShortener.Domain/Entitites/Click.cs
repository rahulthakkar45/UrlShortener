using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Domain.Entitites
{
    public class Click
    {
        public int Id { get; set; }
        public int ShortUrlId { get; set; }
        public DateTime ClickedAt { get; set; }
        public ShortUrl ShortUrl { get; set; }
    }
}
