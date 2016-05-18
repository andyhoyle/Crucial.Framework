using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace Crucial.Framework.Logging.WebApi
{
    public class WebApiExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            var logger = new CrucialLogger();
            logger.Error(context.Exception.Message, context.Exception, context.Request);
        }
    }
}
