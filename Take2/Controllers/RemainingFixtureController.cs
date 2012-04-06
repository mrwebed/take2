using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Take2.Models;

namespace Take2.Controllers
{
    public class RemainingFixtureController : Controller
    {
        private resultsDBcontext db2 = new resultsDBcontext();
        //
        // GET: /RemainingFixtures/

        public ActionResult Index()
        {
            if (db2.RemainingFixtures.Find(1) == null)
            { 
                remainingfixtures(); 
            }
            return View(db2.RemainingFixtures);
        }

        //Generate remaining fixtures
        public void remainingfixtures()
        {
            List<String> teams = db2.TeamLeagueScores.Select(a => a.teamname).ToList();
            foreach (string team in teams)
            {
                List<String> homematchessremainingarray = (from MatchResult in db2.MatchResults
                                                           where MatchResult.hteam == team
                                                           select MatchResult.ateam).ToList();
                //List<String> awaymatchessremainingarray = (from MatchResult in db2.MatchResults
                //                                           where MatchResult.ateam == team
                //                                           select MatchResult.hteam).ToList();
                List<String> remainingteamsnotplayedathome = teams.Except(homematchessremainingarray).ToList();
                //List<String> remainingteamsnotplayedaway = teams.Except(awaymatchessremainingarray).ToList();
                remainingteamsnotplayedathome.Remove(team);
                //RemainingFixture homeRemainingFixture = new RemainingFixture();y
                //List<RemainingFixture> doubledupremaninglist = new List<RemainingFixture>();
                foreach (string awayteamstill2play in remainingteamsnotplayedathome)
                {
                    RemainingFixture derpRemainingFixture = new RemainingFixture();
                    derpRemainingFixture.hteam = team;
                    derpRemainingFixture.ateam = awayteamstill2play;
                    db2.RemainingFixtures.Add(derpRemainingFixture);
                    db2.SaveChanges();
                }
                //foreach (string hometeamstill2play in remainingteamsnotplayedaway)
                //{
                //    RemainingFixture derpRemainingFixture = new RemainingFixture();
                //    derpRemainingFixture.hteam = hometeamstill2play;
                //    derpRemainingFixture.ateam = team;
                //    doubledupremaninglist.Add(derpRemainingFixture);
                //}
                //IEnumerable<RemainingFixture> remainingFList = doubledupremaninglist.Distinct();
                //foreach (RemainingFixture rm in remainingFList)
                //{
                //    db2.RemainingFixtures.Add(rm);
                //    db2.SaveChanges();
                //}
            }
        }
    }
}
