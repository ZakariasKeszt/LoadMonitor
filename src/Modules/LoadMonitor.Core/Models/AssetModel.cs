using System;
using System.Collections.Generic;
using System.Text;

namespace LoadMonitor.Core.Models
{
    public sealed class AssetModel
    {
        public List<Asset> Assets { get; set; } = [];
        public abstract class Asset
        {
            public string ID { get; set; } = "";
            public string Name { get; set; } = "";
            public double PowerKw { get; set; }
            public double CurrentPowerW { get; set; }
            public double RunningHours { get; set; }
            public string ImagePath { get; set; } = "";

            public abstract string Type { get; }
        }

        public sealed class Motor : Asset
        {
            public override string Type => "Motor";
            public int RatedSpeed { get; set; }
            public int Voltage { get; set; }
            public double RatedCurrent {  get; set; }
            public string Efficiency { get; set; } = "";
        }
        
        public sealed class Pump : Asset
        {
            public override string Type => "Pump";
            public double FlowRate { get; set; }
            public double MaxPressure { get; set; }
            public string Medium { get; set; } = "";
        }

    }
}
