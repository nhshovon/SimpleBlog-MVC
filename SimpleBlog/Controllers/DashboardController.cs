﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleBlog.Controllers
{
    [Authorize(Roles = "Developer,Admin,User")]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}