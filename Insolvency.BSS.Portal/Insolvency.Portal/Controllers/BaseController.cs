using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

using Insolvency.Portal.Models;

namespace Insolvency.Portal.Controllers
{
    public class BaseController : Controller
    {
        public BaseController() { }

        protected const string _moratoriumIdKey = "MoratoriumId";
        protected bool HasMoratoriumId() => HttpContext.Session.Keys.Contains(_moratoriumIdKey);
        protected Guid GetMoratoriumId() => new Guid(HttpContext.Session.GetString(_moratoriumIdKey));
        protected void SetMoratoriumId(Guid moratoriumId) => SetMoratoriumId(moratoriumId.ToString());
        protected void SetMoratoriumId(string moratoriumId) => HttpContext.Session.SetString(_moratoriumIdKey, moratoriumId);


        protected bool HasSessionKey(string key) => HttpContext.Session.Keys.Contains(key);
        protected void RemoveSessionByKey(string key) => HttpContext.Session.Remove(key);

        protected void SetSessionObject(string key, object value) => HttpContext.Session.SetString(key, JsonSerializer.Serialize(value));
        protected T GetSessionObject<T>(string key) => JsonSerializer.Deserialize<T>(HttpContext.Session.GetString(key));
        protected void SetTempDataObject(string key, object value) => TempData[key] = JsonSerializer.Serialize(value);
        protected T GetTempDataObject<T>(string key) => JsonSerializer.Deserialize<T>(TempData.Peek(key)?.ToString());


        protected RedirectToActionResult RedirectToHome() => RedirectToAction(nameof(Index));


        protected const string _viewDataJourneyIdKey = "JourneyKey";
        protected const string _viewDataParentJourneyIdKey = "ParentJourneyKey";
        private const string _httpContextItemsJourneyIdKey = "journeyKey";
        private const string _httpContextItemsParentJourneyIdKey = "parentJourneyKey";
        private const string _httpContextItemsActionNameKey = "action";

        protected string GetCurrentJourneyId() => (string)HttpContext.Items[_httpContextItemsJourneyIdKey];
        protected void GenerateNewJourneyId() => HttpContext.Items[_httpContextItemsJourneyIdKey] = Guid.NewGuid().ToString();

        protected string GetCurrentParentJourneyId() => (string)HttpContext.Items[_httpContextItemsParentJourneyIdKey];
        protected string GetCurrentActionName() => (string)HttpContext.Items[_httpContextItemsActionNameKey];
        protected static string GenerateActionJourneyKey(string actionName, string journeyId) => $"{actionName}-{journeyId}";

        private T GetJourneyObject<T>(string journeyActionKey)
        {
            if (journeyActionKey is null)
                return default;

            var valueJson = HttpContext.Session.GetString(journeyActionKey);
            return string.IsNullOrEmpty(valueJson) ? default : JsonSerializer.Deserialize<T>(valueJson);
        }

        protected T GetJourneyObject<T>(string actionOverride = null, string journeyIdOverride = null)
        {
            var journeyId = journeyIdOverride ?? GetCurrentJourneyId();
            var actionName = actionOverride ?? GetCurrentActionName();

            var journeyActionKey = GenerateActionJourneyKey(actionName, journeyId);

            return GetJourneyObject<T>(journeyActionKey);
        }

        protected List<ModelFieldError> GetJourneyModelStateObject()
        {
            var journeyId = GetCurrentJourneyId();
            var actionName = GetCurrentActionName();

            if (actionName is null || journeyId is null)
                return default;

            var journeyActionKey = GenerateActionJourneyKey(actionName + "_ModelState", journeyId);

            return GetJourneyObject<List<ModelFieldError>>(journeyActionKey);
        }

        protected void SetJourneyObject<T>(T value, string actionNameOverride = null)
        {
            var journeyId = GetCurrentJourneyId();
            var actionName = actionNameOverride ?? GetCurrentActionName();

            var journeyActionKey = GenerateActionJourneyKey(actionName, journeyId);

            HttpContext.Session.SetString(journeyActionKey, JsonSerializer.Serialize(value));
        }

        protected void SetJourneyModelStateObject(List<ModelFieldError> value)
        {
            var journeyId = GetCurrentJourneyId();
            var actionName = GetCurrentActionName();

            var journeyActionKey = GenerateActionJourneyKey(actionName + "_ModelState", journeyId);

            HttpContext.Session.SetString(journeyActionKey, JsonSerializer.Serialize(value));
        }

        protected void ClearJourneyModelStateObject()
        {
            var journeyId = GetCurrentJourneyId();
            var actionName = GetCurrentActionName();

            var journeyActionKey = GenerateActionJourneyKey(actionName + "_ModelState", journeyId);

            HttpContext.Session.Remove(journeyActionKey);
        }

        protected RedirectToActionResult StartNewSubJourneyRedirect(string actionName, object routeValues = null)
        {
            var redirectResult = RedirectToAction(actionName, routeValues);

            if (routeValues is null)
                redirectResult.RouteValues = new RouteValueDictionary();

            redirectResult.RouteValues.TryAdd(_httpContextItemsJourneyIdKey, Guid.NewGuid());
            redirectResult.RouteValues.TryAdd(_httpContextItemsParentJourneyIdKey, GetCurrentJourneyId());

            return redirectResult;
        }

        protected RedirectToActionResult ContinueJourneyRedirect(string actionName, object routeValues = null)
        {
            var redirectResult = RedirectToAction(actionName, routeValues);

            if (routeValues is null)
                redirectResult.RouteValues = new RouteValueDictionary();

            redirectResult.RouteValues.TryAdd(_httpContextItemsJourneyIdKey, GetCurrentJourneyId());
            redirectResult.RouteValues.TryAdd(_httpContextItemsParentJourneyIdKey, GetCurrentParentJourneyId());

            return redirectResult;
        }

        protected RedirectToActionResult CompleteSubJourneyRedirect(string actionName, object routeValues = null)
        {
            var redirectResult = RedirectToAction(actionName, routeValues);

            if (routeValues is null)
                redirectResult.RouteValues = new RouteValueDictionary();

            var journeyId = GetCurrentParentJourneyId() ?? GetCurrentJourneyId();

            redirectResult.RouteValues.TryAdd(_httpContextItemsJourneyIdKey, journeyId);

            return redirectResult;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            HttpContext.Request.Query.TryGetValue(_httpContextItemsJourneyIdKey, out var journeyId);
            HttpContext.Request.Query.TryGetValue(_httpContextItemsParentJourneyIdKey, out var parentJourneyId);

            var actionName = context.ActionDescriptor.RouteValues[_httpContextItemsActionNameKey];

            HttpContext.Items.Add(_httpContextItemsJourneyIdKey, journeyId.DefaultIfEmpty()?.FirstOrDefault());
            HttpContext.Items.Add(_httpContextItemsParentJourneyIdKey, parentJourneyId.DefaultIfEmpty()?.FirstOrDefault());
            HttpContext.Items.Add(_httpContextItemsActionNameKey, actionName);

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var journeyId = GetCurrentJourneyId();
            var parentJourneyId = GetCurrentParentJourneyId();

            ViewData[_viewDataJourneyIdKey] = journeyId;
            ViewData[_viewDataParentJourneyIdKey] = parentJourneyId;

            base.OnActionExecuted(context);
        }
    }
}
