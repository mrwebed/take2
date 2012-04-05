using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Take2.Models
{
    public class TeamLeagueScore
    {
        public int TeamLeagueScoreId { get; set; }
        public string teamname { get; set; }
        public int points { get; set; }
        public int goalfor { get; set; }
        public int goalagainst { get; set; }
    }
}