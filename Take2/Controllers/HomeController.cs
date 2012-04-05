using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Take2.Models;
using System.Data.Entity;


namespace Take2.Controllers
{
    public class HomeController : Controller
    {

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }
    }
}