using System;
using System.Collections.Generic;
using System.Text;
using OrchardCore.ContentManagement;
namespace LoadMonitor.Core.Models
{
    //Basic content part definition for the creation of Asset content type
    public class AssetPart : ContentPart
    {
        public double RatedPowerKw { get; set; }
        public double CurrentPowerW { get; set; }
        public double RunningHours { get; set; }
    }
}
