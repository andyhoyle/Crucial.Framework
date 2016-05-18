using Crucial.Framework.Logging.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Tracing;

namespace Crucial.Framework.Logging.WebApi
{
    public class GlobalExceptionAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public GlobalExceptionAttribute()
        {
            _logger = new CrucialLogger();
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            _logger.LogException(context.Exception);
            
            var exceptionType = context.Exception.GetType();

            if (exceptionType == typeof(ValidationException))
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(context.Exception.Message), ReasonPhrase = "ValidationException", };
                throw new HttpResponseException(resp);

            }
            else if (exceptionType == typeof(UnauthorizedAccessException))
            {
                throw new HttpResponseException(context.Request.CreateResponse(HttpStatusCode.Unauthorized, new ServiceStatus() { StatusCode = (int)HttpStatusCode.Unauthorized, StatusMessage = "UnAuthorized", ReasonPhrase = "UnAuthorized Access" }));
            }
            else if (exceptionType == typeof(ApiException))
            {
                var webapiException = context.Exception as ApiException;
                if (webapiException != null)
                    throw new HttpResponseException(context.Request.CreateResponse(webapiException.HttpStatus, new ServiceStatus() { StatusCode = webapiException.ErrorCode, StatusMessage = webapiException.ErrorDescription, ReasonPhrase = webapiException.ReasonPhrase }));
            }
            else if (exceptionType == typeof(ApiBusinessException))
            {
                var businessException = context.Exception as ApiBusinessException;
                if (businessException != null)
                    throw new HttpResponseException(context.Request.CreateResponse(businessException.HttpStatus, new ServiceStatus() { StatusCode = businessException.ErrorCode, StatusMessage = businessException.ErrorDescription, ReasonPhrase = businessException.ReasonPhrase }));
            }
            else if (exceptionType == typeof(ApiDataException))
            {
                var dataException = context.Exception as ApiDataException;
                if (dataException != null)
                    throw new HttpResponseException(context.Request.CreateResponse(dataException.HttpStatus, new ServiceStatus() { StatusCode = dataException.ErrorCode, StatusMessage = dataException.ErrorDescription, ReasonPhrase = dataException.ReasonPhrase }));
            }
            else
            {
                throw new HttpResponseException(context.Request.CreateResponse(HttpStatusCode.InternalServerError, new ServiceStatus() { StatusCode = 500, StatusMessage = context.Exception.Message, ReasonPhrase = context.Exception.StackTrace }));
            }
        }
    }
}
