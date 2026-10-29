using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoadMonitor.Core.Models;
using LoadMonitor.Core.Services;

namespace LoadMonitor.Core.Controllers;

public sealed class HomeController : Controller
{
    public ActionResult Index()
    {
        return View(new AssetModel { Assets = AssetStorage.Assets });
    }
}
