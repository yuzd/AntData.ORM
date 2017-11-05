using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AntData.ORM.Data;
using DbModels.Mysql;
using Microsoft.AspNetCore.Mvc;
using netcore2web.Models;

namespace netcore2web.Controllers
{
    public class HomeController : Controller
    {
        public DbContext<TestormEntitys> DB { get; set; }
        public HomeController(DbContext<TestormEntitys> tables)
        {
            DB = tables;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            var person = DB.Tables.People.FirstOrDefault();
            //DB.UseTransaction(con=>
            //{
            //})
            ViewData["Message"] = person.Name;

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
