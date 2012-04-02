using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Take2.Models
{
    public class dbInitalizer : DropCreateDatabaseAlways<resultsDBcontext>
    {
        protected override void Seed(resultsDBcontext context)
        {
            //var mresulseed = new List<MatchResult>
            //{
            //new MatchResult
            //{
            //    date = "somedate",
            //    ateam = "someawayteam",
            //    hteam = "somehometeam",
            //    hteamres = "somehometeamWLD",
            //    ateamgoal = "somegoals",
            //    hteamgoal = "Somemoregoals",
            //    idnum = "someID",
            //    //idtrick = "BWAHAH"
            //};
            //};
            //mresulseed.ForEach(a => context.MatchResults.Add(a));
        }
    }
}