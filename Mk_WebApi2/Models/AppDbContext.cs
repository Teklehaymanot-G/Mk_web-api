using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mk_WebApi2.Model;
using Mk_WebApi2.Models;

namespace Mk_WebApi2.Model
{
    public class AppDbContext: DbContext
    {
        public AppDbContext()
        {

        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Mk_WebApi2.Models.MaekelModel> maekel { get; set; }
        public DbSet<Mk_WebApi2.Models.KifilModel> kifil { get; set; }
        public DbSet<Mk_WebApi2.Models.Sub_KifilModel> sub_kifil { get; set; }
        public DbSet<Mk_WebApi2.Models.AbalatModel> abalat { get; set; }
        public DbSet<Mk_WebApi2.Models.AccountModel> account { get; set; }

    }
}
