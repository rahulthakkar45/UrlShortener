using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Interfaces;
using UrlShortener.Domain.Entitites;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories
{
    public class ClickRepository : IClickRepository
    {
        private readonly UrlShortenerDbContext _context;

        public ClickRepository(UrlShortenerDbContext context)
        {
            _context = context;
        }

        public async Task Add(Click click)
        {
            await _context.Clicks.AddAsync(click);
            await _context.SaveChangesAsync();
        }
    }
}
