using SWE1HttpServer.Core.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SWE1HttpServer.Core.Listener
{
    public class HttpListener : IListener
    {
        private readonly TcpListener listener;

        public HttpListener(IPAddress address, int port)
        {
            listener = new TcpListener(address, port);
        }


        public IClient AcceptClient()
        {
            var client = listener.AcceptTcpClient();
            return new HttpClient(client);
        }

        public void Start()
        {
            listener.Start();
        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}
