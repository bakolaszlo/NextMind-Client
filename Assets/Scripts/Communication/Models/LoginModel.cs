using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Communication.Models
{
    internal class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public float[] SensorData { get; set; }
    }
}
