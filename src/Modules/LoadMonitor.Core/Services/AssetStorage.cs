using System;
using System.Collections.Generic;
using System.Text;
using LoadMonitor.Core.Models;

namespace LoadMonitor.Core.Services
{
    public static class AssetStorage
    {
        public static readonly List<AssetModel.Asset> Assets = [
                new AssetModel.Motor
                {
                    ID = "001",
                    Name = "Motor",
                    PowerKw = 20,
                    CurrentPowerW = 1500,
                    RunningHours = 1210,
                    ImagePath = "~/Images/motor1.jpg",
                    RatedSpeed = 1000,
                    Voltage = 400,
                    RatedCurrent = 50,
                    Efficiency = "IE3",
                },

                new AssetModel.Motor
                {
                    ID = "002",
                    Name = "Motor2",
                    PowerKw = 50,
                    CurrentPowerW = 150,
                    RunningHours = 120,
                    ImagePath = "~/Images/motor2.jpg",
                    RatedSpeed = 1500,
                    Voltage = 400,
                    RatedCurrent = 125,
                    Efficiency = "IE3",
                },

                new AssetModel.Pump
                {
                    ID = "003",
                    Name = "Pump",
                    PowerKw = 30,
                    CurrentPowerW = 24000,
                    RunningHours = 1230,
                    ImagePath = "~/Images/Pump.jpg",
                    FlowRate = 300,
                    MaxPressure = 20,
                    Medium = "Wata"
                },
            ];
    }
}
