using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Take2.Models
{
    public class MatchResult
    {
        public int ID { get; set; }
        public DateTime date { get; set; }
        public string ateam { get; set; }
        public string hteam { get; set; }
        public string hteamres { get; set; }
        public int ateamgoal { get; set; }
        public int hteamgoal { get; set; }
        public string idnum { get; set; }
        //public string idtrick { get; set; }
        public DateTime scrapetime { get; set; }
    }
}