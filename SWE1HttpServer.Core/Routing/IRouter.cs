using SWE1HttpServer.Core.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWE1HttpServer.Core.Routing
{
    public interface IRouter
    {
        IRouteCommand Resolve(RequestContext request);
    }
}
