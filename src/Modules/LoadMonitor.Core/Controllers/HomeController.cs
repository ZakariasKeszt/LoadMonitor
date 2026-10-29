using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoadMonitor.Core.Models;

namespace LoadMonitor.Core.Controllers;

public sealed class HomeController : Controller
{
    public ActionResult Index()
    {
        var model = new AssetModel
        {
            Assets = 
            [
                new AssetModel.Asset
                {                
                    Name = "Motor",
                    PowerKw = 30,
                    CurrentPowerW = 1500,
                    RunningHours = 1210,
                    ImagePath = "~/Images/motor1.jpg",
                },

                new AssetModel.Asset
                {
                    Name = "Motor2",
                    PowerKw = 50,
                    CurrentPowerW = 150,
                    RunningHours = 120,
                    ImagePath = "~/Images/motor2.jpg",
                },
                
                new AssetModel.Asset
                {
                    Name = "Pump",
                    PowerKw = 30,
                    CurrentPowerW = 24000,
                    RunningHours = 1230,
                    ImagePath = "~/Images/Pump.jpg",
                },
            ],

        };
        return View(model);
    }
}
