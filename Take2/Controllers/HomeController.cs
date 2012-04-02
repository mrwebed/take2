using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using Take2.Models;

namespace Take2.Controllers
{
    public class HomeController : Controller
    {
        resultsDBcontext dbfirstfuntimes = new resultsDBcontext();
        //public static new MatchResult nodeBeingProcessed.ateam;
        //
        // GET: /Home/

        public ActionResult Index()
        {
            HtmlDocument firstscrape = scraperOutput();
            dubNodeSelection(firstscrape);
            var theseresults = dbfirstfuntimes.MatchResults.ToList();
            return View(theseresults);
        }

        //Data scraper, gets the Beeb results page, placing into a 
        public HtmlDocument scraperOutput ()
        {
            string url = @"http://www.bbc.co.uk/sport/football/results/partial/competition-118996114";
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument scrapeResults = new HtmlDocument
            {
                OptionUseIdAttribute = true,
            //    OptionFixNestedTags = true,
            //    OptionAutoCloseOnEnd = true,
            //    OptionOutputAsXml = true,
            //    OptionOutputUpperCase = true
            };
            scrapeResults = htmlWeb.Load(url);
            
            return scrapeResults;
        }

        /////Convert the scraped into a hmtlnode/nodelist & add to the DB
        ////START OF MUTLIPLE NODES
        public void multiNodeSelection (HtmlDocument htmlDoc)
        {
            
            HtmlNodeCollection fullmatchresults = htmlDoc.DocumentNode.SelectNodes("//tr[@id]");
            foreach (HtmlNode mtchrslt in fullmatchresults)
            {
                MatchResult dblineobjmtchrslt = matchResultProcessed(mtchrslt);
                //// converted node added to the DB here
                dbfirstfuntimes.MatchResults.Add(dblineobjmtchrslt);
                dbfirstfuntimes.SaveChanges();
            }
        }

        ////WORKING WITH A SINGLE NODE
        public void singNodeSelection(HtmlDocument htmlDoc)
        { 
            HtmlNode fullmatchresult = htmlDoc.DocumentNode.SelectSingleNode("//tr[@id]");
            MatchResult dblineobjmtchrslt = matchResultProcessed(fullmatchresult);
            //// converted node added to the DB here
            dbfirstfuntimes.MatchResults.Add(dblineobjmtchrslt);
            dbfirstfuntimes.SaveChanges();
            //return;
        }

        //WORKING WITH 2 NODES
        public void dubNodeSelection(HtmlDocument htmlDoc)
        {
            HtmlNodeCollection fullmatchresult = htmlDoc.DocumentNode.SelectNodes("//tr[@id]");
            int n = 0;
            while (n != 2)
            {
                HtmlNode noderesult = fullmatchresult.ElementAt(n);
                MatchResult dblineobjmtchrslt = matchResultProcessed(noderesult);
                //// converted node added to the DB here
                dbfirstfuntimes.MatchResults.Add(dblineobjmtchrslt);
                dbfirstfuntimes.SaveChanges();
                n++;
                //return;
            }
        }

        //Process a HtmlNode into a MatchResult
        public MatchResult matchResultProcessed (HtmlNode matchResultHtmlNode)
        {
            MatchResult nodeBeingProcessed = new MatchResult();
            HtmlNode matchResultHtmlNode1 = matchResultHtmlNode;

            string idmess = matchResultHtmlNode1.SelectSingleNode("//tr[@id]").Id;
            nodeBeingProcessed.idnum = idmess.Replace("match-row-", "").Trim();

            string score = matchResultHtmlNode1.SelectSingleNode("//abbr[@title='Score']").InnerText;
            string[] teamscores = score.Split('-');
            nodeBeingProcessed.hteamgoal = teamscores[0].Trim();
            nodeBeingProcessed.ateamgoal = teamscores[1].Trim();
            // if (nodeBeingProcessed.hteamgoal>nodeBeingProcessed.ateamgoal) {}
            nodeBeingProcessed.hteam = matchResultHtmlNode1.SelectSingleNode("//p[(@class='team-home teams')]").InnerText.Trim();
            nodeBeingProcessed.ateam = matchResultHtmlNode1.SelectSingleNode("//p[(@class='team-away teams')]").InnerText.Trim();
            nodeBeingProcessed.date = matchResultHtmlNode1.SelectSingleNode("//td[(@class='match-date')]").InnerText.Trim();
            return nodeBeingProcessed;
        }

        //public MatchResult matchResultProcessed(HtmlNode matchResultHtmlNode, int n)
        //{
        //    MatchResult nodeBeingProcessed = new MatchResult();
        //    //new HtmlNode(1,,0) matchResultHtmlNode1;
        //        //= matchResultHtmlNode;

        //    string idmess = matchResultHtmlNode.SelectSingleNode("//tr[@id]").Id;
        //    nodeBeingProcessed.idnum = idmess.Replace("match-row-", "").Trim();

        //    string score = matchResultHtmlNode.SelectSingleNode("//abbr[@title='Score']").InnerText;
        //    string[] teamscores = score.Split('-');
        //    nodeBeingProcessed.hteamgoal = teamscores[0].Trim();
        //    nodeBeingProcessed.ateamgoal = teamscores[1].Trim();
        //    // if (nodeBeingProcessed.hteamgoal>nodeBeingProcessed.ateamgoal) {}
        //    nodeBeingProcessed.hteam = matchResultHtmlNode.SelectSingleNode("//p[(@class='team-home teams')]").InnerText.Trim();
        //    nodeBeingProcessed.ateam = matchResultHtmlNode.SelectSingleNode("//p[(@class='team-away teams')]").InnerText.Trim();
        //    nodeBeingProcessed.date = matchResultHtmlNode.SelectSingleNode("//td[(@class='match-date')]").InnerText.Trim();
        //    return nodeBeingProcessed;
        //}
       
       
    }
}
