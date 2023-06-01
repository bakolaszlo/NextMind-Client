using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Communication.Models
{
    internal class Ping
    {
        public int Status { get; set; }
        public byte[] position { get; set; }
        public string SessionId { get; set; }
    }
}
