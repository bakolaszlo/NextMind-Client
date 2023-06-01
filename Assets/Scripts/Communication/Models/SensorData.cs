using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Communication.Models
{
    internal class SensorData
    {
        public float[] SensorValues { get; set; }
        public DateTime RecordedTime { get; set; }
    }
}
