using MyMovieWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IOrderServices
    {
        Task CreateOrder (Order order);

    }
}
