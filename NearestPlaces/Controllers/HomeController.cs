using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NearestPlaces.Database;

namespace NearestPlaces.Controllers
{
    public class HomeController : Controller
    {
        private PlaceDB db = new PlaceDB();

        public async Task<ActionResult> Index()
        {
            ViewBag.Title = "Home Page";
            return View(await db.Places.ToListAsync());
        }
    }
}
