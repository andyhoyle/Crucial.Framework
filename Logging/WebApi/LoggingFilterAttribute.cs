using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Tracing;

namespace Crucial.Framework.Logging.WebApi
{
    public class LoggingFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger _logger;

        public LoggingFilterAttribute()
        {
            _logger = new CrucialLogger();
        }

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            
            _logger.Trace("Controller : " + filterContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine + "Action : " + filterContext.ActionDescriptor.ActionName, "JSON", filterContext.ActionArguments, null, filterContext.Request);
        }
    }
}
