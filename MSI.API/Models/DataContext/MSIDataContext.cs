using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MSI.API.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSI.API.Models.DataContext
{
    public class MSIDataContext:IdentityDbContext<ApplicationUserModel>
    {
        public MSIDataContext(DbContextOptions<MSIDataContext> options):base(options)
        {            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
