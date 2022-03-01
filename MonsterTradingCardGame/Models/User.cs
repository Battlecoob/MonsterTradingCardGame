using SWE1HttpServer.Core.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Models
{
    public class Package
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public List<string> CardIds { get; set; }
    }
}
