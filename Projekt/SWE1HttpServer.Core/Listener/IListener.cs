using SWE1HttpServer.Core.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE1HttpServer.Core.Listener
{
    public interface IListener
    {
        IClient AcceptClient();
        void Start();
        void Stop();
    }
}
