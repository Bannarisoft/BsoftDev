using Microsoft.EntityFrameworkCore;
using BSOFT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) 
        : base(dbContextOptions) 
        {           
        }
        public DbSet<User> User { get; set; }

    }
}