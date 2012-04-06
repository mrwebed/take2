using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Take2.Models
{
    public class resultsDBcontext : DbContext
    {
        public DbSet<MatchResult> MatchResults { get; set; }
        public DbSet<TeamLeagueScore> TeamLeagueScores { get; set; }
        public DbSet<RemainingFixture> RemainingFixtures { get; set; }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //}
    }
}