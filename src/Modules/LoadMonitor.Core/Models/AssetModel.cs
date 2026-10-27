using System;
using System.Collections.Generic;
using System.Text;

namespace LoadMonitor.Core.Models
{
    internal class AssetModel
    {
        public List<Asset> Assets { get; set; } = [];
        public sealed class Asset
        {
            public string Name { get; set; } = "";
            public double PowerKw { get; set; }
            public double CurrentPowerW { get; set; }
            public double RunningHours { get; set; }
            public string ImagePath { get; set; } = "";
        }
    }
}
