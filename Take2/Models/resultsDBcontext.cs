using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
//using Take2.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Take2.Models
{
    public class resultsDBcontext : DbContext
    {
        public DbSet<MatchResult> MatchResults { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //}
    }
}