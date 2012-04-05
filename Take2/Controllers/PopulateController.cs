using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using Take2.Models;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Take2.Controllers
{
    public class PopulateController : Controller
    {
        private resultsDBcontext dbfirstfuntimes = new resultsDBcontext();
        
        //
        // GET: /Populate/Index/
        public ActionResult Index()
        {
            Scrapetimeprocessing();
            if (dbfirstfuntimes.MatchResults.Find(1) != null)
            {
                var scrapeRes1 = dbfirstfuntimes.MatchResults;
                return View(scrapeRes1);
            }
            return View();
        }
        //
        // GET: /Populate/Scrapeee
        public ActionResult Scrapee()
        {
            HtmlDocument scrapedHtmlPage = scraperOutput();
            if (TempData["htmlscaprefail"] != null) { return RedirectToAction("Error"); }
            //if database isnt empty and if the first results matchID is the same as the first results matchID leave the db alone
            if (dbfirstfuntimes.MatchResults.Find(1) != null && scrapedHtmlPage.DocumentNode.SelectSingleNode("//tr[@id]").Id.Replace("match-row-", "").Trim()
                    == dbfirstfuntimes.MatchResults.Find(1).idnum)
            { //db exists and is up2date
                var scrapeRes1 = dbfirstfuntimes.MatchResults;
                ViewBag.comefrom = true;
                return View(scrapeRes1);
            }
            else
            { //db doesnt exist OR is out of date =>- create it
                List<MatchResult> scrapeRes2 = ActioningTheScrape(scrapedHtmlPage);
                ViewBag.comefrom = false;
                return View(scrapeRes2);
            }
        }
        //
        // GET: /Populate/Error
        public ActionResult Error()
        {   
            return View();
        }
        //
        //Proccess the scraped html, times the Db creation
        public List<MatchResult> ActioningTheScrape(HtmlDocument scrapedDoc)
        {
            DateTime theNow = DateTime.Now;
            ViewBag.thenow = theNow.TimeOfDay;
            multiNodeSelection(scrapedDoc);
            ViewBag.time3 = DateTime.Now.Subtract(theNow).TotalSeconds;
            List<MatchResult> theseresults = dbfirstfuntimes.MatchResults.ToList();
            ViewBag.thenownow = (DateTime.Now).TimeOfDay;
            return theseresults;
        }
        //
        //Last Scrape : date/time details got from db
        public void Scrapetimeprocessing()
        {
            //DateTime lastScraped = new DateTime;
            if (dbfirstfuntimes.MatchResults.Find(1) != null)
            {
            DateTime lastScraped = dbfirstfuntimes.MatchResults.Find(1).scrapetime;
            ViewBag.lastscrapeddate = lastScraped;
            ViewBag.lastscrapedinhours = (DateTime.Now - lastScraped).TotalHours.ToString("F");
            }
            else
            {ViewBag.lastscrapenull = true; }
            //
            //try { 
            //DateTime lastScraped = dbfirstfuntimes.MatchResults.Find(1).scrapetime;
            //ViewBag.lastscrapeddate = lastScraped;
            //ViewBag.lastscrapedinhours = (DateTime.Now - lastScraped).TotalHours.ToString("F");
            //}
            //catch (System.ArgumentNullException)  //catches when DB doesnt exist
            //{  
            //    ViewBag.lastscrapenull = true; 
            }
        }

        //
        //Data scraper, gets the Beeb results page, placing into a 
        public HtmlDocument scraperOutput ()
        {
            string url = @"http://www.bbc.co.uk/sport/football/results/partial/competition-118996114";
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument scrapeResults = new HtmlDocument {OptionUseIdAttribute = true};
            try
            {
                scrapeResults = htmlWeb.Load(url);
                double a = htmlWeb.RequestDuration;
                ViewBag.loadingTime = a / 1000;
                return scrapeResults;
            }
                //Exception handling for a scrape fail (wrong address, timeout etc)
            catch (System.Net.WebException scrapefail)
            {
                TempData["htmlscaprefail"] = scrapefail.Message;
                return null;
            }
        }

        //
        /////Convert the scraped into a hmtlnode/nodelist, format & add to the DB
        ////START OF MUTLIPLE NODES
        public void multiNodeSelection(HtmlDocument htmlDoc)
        {

            HtmlNodeCollection fullmatchresults = htmlDoc.DocumentNode.SelectNodes("//tr[@id]");
            foreach (HtmlNode mtchrslt in fullmatchresults)
            {
                MatchResult dblineobjmtchrslt = matchResultProcessed(mtchrslt);
                //// converted node added to the DB here
                if (dblineobjmtchrslt != null)
                {
                    dbfirstfuntimes.MatchResults.Add(dblineobjmtchrslt);
                    dbfirstfuntimes.SaveChanges();
                }
            }
        }

        //////WORKING WITH A SINGLE NODE
        //public void singNodeSelection(HtmlDocument htmlDoc)
        //{ 
        //    HtmlNode fullmatchresult = htmlDoc.DocumentNode.SelectSingleNode("//tr[@id]");
        //    MatchResult dblineobjmtchrslt = matchResultProcessed(fullmatchresult);
        //    //// converted node added to the DB here
        //    dbfirstfuntimes.MatchResults.Add(dblineobjmtchrslt);
        //    dbfirstfuntimes.SaveChanges();
        //    //return;
        //}

        ////WORKING WITH 2 NODES
        //public void dubNodeSelection(HtmlDocument htmlDoc)
        //{
        //    HtmlNodeCollection fullmatchresult = htmlDoc.DocumentNode.SelectNodes("//tr[@id]");
        //    int n = 0;
        //    while (n != 2)
        //    {
        //        HtmlNode noderesult = fullmatchresult.ElementAt(n);
        //        MatchResult dblineobjmtchrslt = matchResultProcessed(noderesult);
        //        //// converted node added to the DB here
        //        if (dblineobjmtchrslt != null)
        //        {
        //            dbfirstfuntimes.MatchResults.Add(dblineobjmtchrslt);
        //            dbfirstfuntimes.SaveChanges();
        //        }
        //        n++;
        //    }
        //}

        //Process a HtmlNode into a MatchResult
        public MatchResult matchResultProcessed (HtmlNode matchResultHtmlNode)
        {
            MatchResult nodeBeingProcessed = new MatchResult();
            //match ID
            string idmess = matchResultHtmlNode.Id;
            nodeBeingProcessed.idnum = idmess.Replace("match-row-", "").Trim();
            //The score
            if (matchResultHtmlNode.SelectSingleNode("descendant::abbr[@title='Score']") != null)
            {
                string[] score = matchResultHtmlNode.SelectSingleNode("descendant::abbr[@title='Score']").InnerText.Split('-');
                nodeBeingProcessed.hteamgoal = int.Parse(score[0].Trim());
                nodeBeingProcessed.ateamgoal = int.Parse(score[1].Trim());
                //Hometeam W/L/D?
                if (nodeBeingProcessed.hteamgoal > nodeBeingProcessed.ateamgoal)
                {
                    nodeBeingProcessed.hteamres = "W";
                }
                else if (nodeBeingProcessed.hteamgoal == nodeBeingProcessed.ateamgoal)
                {
                    nodeBeingProcessed.hteamres = "D";
                }
                else
                {
                    nodeBeingProcessed.hteamres = "L";
                }
            }
            else if (matchResultHtmlNode.SelectSingleNode("descendant::abbr[@title='Postponed']") != null)
            { return null; }
            else 
            {
                nodeBeingProcessed.hteamgoal = 0;
                nodeBeingProcessed.ateamgoal = 0;
                nodeBeingProcessed.hteamres = "IN PROGRESS/TO PLAY";
            }
            
            //team names
            nodeBeingProcessed.hteam = matchResultHtmlNode.SelectSingleNode("descendant::span[(@class='team-home teams')]").InnerText.Trim();
            nodeBeingProcessed.ateam = matchResultHtmlNode.SelectSingleNode("descendant::span[(@class='team-away teams')]").InnerText.Trim();
            //Matchdate string-> dddd d MMMM yyyy -> datetime
            string value = matchResultHtmlNode.SelectSingleNode("ancestor::table").Element("caption").InnerText.Substring(38);
            DateTime dt;
            DateTime.TryParseExact(Regex.Replace(value, @"(\w+ \d+)\w+ (\w+ \d+)", "$1 $2"),"dddd d MMMM yyyy",
                DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out dt);
            nodeBeingProcessed.date = dt;

            //DateTime.Parse(
            //= DateTime.Parse(matchdateholder.Substring(matchdateholder.LastIndexOf("day")))'


            //nodeBeingProcessed.date = DateTime.Parse(
                
                //("descendant::td[(@class='match-date')]").InnerText.Trim());
            //scrape date
            nodeBeingProcessed.scrapetime = DateTime.Now;
            return nodeBeingProcessed;
        }
    }
}