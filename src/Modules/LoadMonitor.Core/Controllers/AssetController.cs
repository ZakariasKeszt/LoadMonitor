using LoadMonitor.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using LoadMonitor.Core.Services;

namespace LoadMonitor.Core.Controllers
{
    public sealed class AssetController : Controller
    {

        public IActionResult Details(int id)
        {
            if (id < 0 || id >= AssetStorage.Assets.Count)
            {
                return NotFound();
            }
            return View(AssetStorage.Assets[id]);
        }
    }
}
