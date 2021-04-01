using System;
using System.Text.Json;
using Insolvency.Common.Exceptions;
using Insolvency.IntegrationAPI.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Insolvency.IntegrationAPI
{
    public class ExceptionHandling : IExceptionFilter
    {
        public string OperationId => System.Diagnostics.Activity.Current.RootId;
        public bool EnableErrorMessageResponse { get; set; }
        public ExceptionHandling(bool enableErrorMessageResponse)
        {
            EnableErrorMessageResponse = enableErrorMessageResponse;
        }        

        public void OnException(ExceptionContext context)
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<ExceptionHandling>>();

            logger.LogError($"{context.Exception.ToString()}");

            if (typeof(HttpResponseException).IsAssignableFrom(context.Exception.GetType()))
            {
                var httpResponseException = (HttpResponseException)context.Exception;
                SetExceptionResponse(context, httpResponseException.StatusCode, httpResponseException.Message);
                return;
            }

            var exception = context.Exception as Simple.OData.Client.WebRequestException;

            var statusCode = 500;

            if (exception == null)
            {
                SetExceptionResponse(context, statusCode, context.Exception.Message);

                return;
            }

            logger.LogError($"{exception.Response}");
            statusCode = (int)exception.Code;
            var errorMessage = exception.Response;

            try
            {
                var response = JsonSerializer.Deserialize<DynamicsError>(exception.Response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                errorMessage = response.Error.Message;
                statusCode = (int)exception.Code;

                var customError = response.GetCustomError();

                errorMessage = customError.Message;
                statusCode = customError.ErrorCode;
            }
            catch(Exception ex)
            {
                logger.LogInformation(ex, "Failed to parse exception message");
            }

            SetExceptionResponse(context, statusCode, errorMessage);
        }

        private void SetExceptionResponse(ExceptionContext context, int statusCode, string errorMessage)
        {
            context.ModelState.AddModelError("OperationId", OperationId);

            if (EnableErrorMessageResponse)
            {
                context.ModelState.AddModelError("ErrorMessage", errorMessage);
            }

            var errors = new ValidationProblemDetails(context.ModelState);

            errors.Title = ("One or more errors occured.");

            context.Result = new ObjectResult(errors)
            {
                StatusCode = statusCode,
            };
        }
    }
}
