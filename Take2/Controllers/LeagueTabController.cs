using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Take2.Models;

namespace Take2.Controllers
{
    public class LeagueTabController : Controller
    {
        //
        // GET: /LeagueTab/
        private resultsDBcontext db = new resultsDBcontext();
        
        public ActionResult Index()
        {
            if(db.TeamLeagueScores.Find(1) == null)
            //try { db.TeamLeagueScores.Find(1); }//{ tablemaker(); }
            //catch (System.Data.EntityCommandExecutionException)
            { tablemaker();
            restoftablegenerater();
            }
            return View(db.TeamLeagueScores.OrderByDescending(TeamLeagueScore => TeamLeagueScore.points).
                ThenByDescending(TeamLeagueScore => TeamLeagueScore.goaldifference).
                ThenByDescending(TeamLeagueScore => TeamLeagueScore.goalfor));
        }
        //DB generater


        //Make the table :O make the DB possibly
        public void tablemaker()
        {
            //IEnumerable<MatchResult> 
            var letsgogetalistofteamnames = db.MatchResults.Select(p => p.hteam).Distinct();
                //(from hteam in db.MatchResults select hteam).Distinct();
            foreach (var teamnameitem in letsgogetalistofteamnames)
            {
                TeamLeagueScore abrandnewteam = new TeamLeagueScore();
                abrandnewteam.teamname = teamnameitem;
                db.TeamLeagueScores.Add(abrandnewteam);
                db.SaveChanges();
            }
            return;
        }

        public void restoftablegenerater()
        {
            var thelistofteams = db.TeamLeagueScores.ToList();
            foreach (TeamLeagueScore teamitem in thelistofteams)
            {
                //teamitem.points 
                List<String> homeresultsarray = (from MatchResult in db.MatchResults
                                                 where MatchResult.hteam == teamitem.teamname
                                                 select MatchResult.hteamres).ToList();
                List<String> awayresultsarray = (from MatchResult in db.MatchResults
                                                 where MatchResult.ateam == teamitem.teamname
                                                 select MatchResult.hteamres).ToList();
                int i = 0;
                foreach (var pointsvarresult in homeresultsarray)
                {
                    if (pointsvarresult == "W")
                    {
                        i = i + 3;
                    }
                    else if (pointsvarresult == "D")
                    { i++; }
                }
                //This is back 2 front cos its referenced to the home teams result
                // and is a list of THIS teams away matches
                foreach (var pointsvarresult2 in awayresultsarray)
                {
                    if (pointsvarresult2 == "L")
                    {
                        i = i + 3;
                    }
                    else if (pointsvarresult2 == "D")
                    { i++; }
                }
                teamitem.points = i;
                //teamitem.goalsfor
                int homegoalstotal = (from MatchResult in db.MatchResults
                                                 where MatchResult.hteam == teamitem.teamname
                                                 select MatchResult.hteamgoal).ToArray().Sum();
                int awaygoalstotal = (from MatchResult in db.MatchResults
                                                 where MatchResult.ateam == teamitem.teamname
                                                 select MatchResult.ateamgoal).ToArray().Sum();
                teamitem.goalfor = homegoalstotal + awaygoalstotal;
                //teamitem.goalagainst
                int homegoalsagainsttotal = (from MatchResult in db.MatchResults
                                      where MatchResult.hteam == teamitem.teamname
                                      select MatchResult.ateamgoal).ToArray().Sum();
                int awaygoalsagainsttotal = (from MatchResult in db.MatchResults
                                      where MatchResult.ateam == teamitem.teamname
                                      select MatchResult.hteamgoal).ToArray().Sum();
                teamitem.goalagainst = homegoalsagainsttotal + awaygoalsagainsttotal;
                //teamitem.goaldifference
                teamitem.goaldifference = teamitem.goalfor - teamitem.goalagainst;

                db.SaveChanges();
            }
          
        }

        //Generate remaining fixtures

    }
}
