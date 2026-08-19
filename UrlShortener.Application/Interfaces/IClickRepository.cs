using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Domain.Entitites;

namespace UrlShortener.Application.Interfaces
{
    public interface IClickRepository
    {
        Task Add(Click click);
    }
}
