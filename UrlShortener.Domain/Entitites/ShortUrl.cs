using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Domain.Entitites
{
    public class ShortUrl
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string OriginalUrl { get; set; }
        public string ShortCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; } = false;
        public User User { get; set; }
        public ICollection<Click> Clicks { get; set; }
    }
}
