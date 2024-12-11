using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
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
        public DbSet<User> Users { get; set; }
        public DbSet<Unit> Unit { get; set; } 
        public DbSet<Entity> Entity { get; set; } 

    }
}