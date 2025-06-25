using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Techno_Home.Models;

namespace Techno_Home.Data
{
    public class Techno_HomeContext : DbContext
    {
        public Techno_HomeContext (DbContextOptions<Techno_HomeContext> options)
            : base(options)
        {
        }

        public DbSet<Techno_Home.Models.Product> Product { get; set; } = default!;
    }
}
