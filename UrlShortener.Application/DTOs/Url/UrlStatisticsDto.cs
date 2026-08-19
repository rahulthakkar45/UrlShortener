using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.DTOs.Url
{
    public class UrlStatisticsDto
    {
        public int TotalClicks { get; set; }
        public int ClicksToday { get; set; }
        public int clicksLast7Days { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
