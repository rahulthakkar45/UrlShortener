using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.DTOs.Url
{
    public class UrlListDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Search { get; set; }
        public string SortBy { get; set; } = "CreatedAt";
        public string Status { get; set; }

    }
}
