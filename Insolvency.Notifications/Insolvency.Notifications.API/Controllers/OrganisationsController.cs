using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insolvency.Interfaces.Notifications;
using Insolvency.Notifications.API.Extensions;
using Insolvency.Notifications.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Insolvency.Notifications.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Policy = Constants.Auth.HasSelectedOrganisationPolicy)]
    public class OrganisationsController : ControllerBase
    {
        public IMessagingClient Client { get; }
        public bool IsResetEnabled { get; }
        public INotificationRepository Repository { get; }

        public OrganisationsController(INotificationRepository repository, IMessagingClient client, IConfiguration configuration)
        {
            this.IsResetEnabled = configuration.GetValue<bool>("EnableReset");
            this.Repository = repository;
            this.Client = client;
        }

        [HttpGet]
        [Route("{id}/Pending")]
        public async Task<ActionResult<IEnumerable<NotificationViewModel>>> PendingNotifications(Guid id)
        {
            if (id != HttpContext.GetOrganisationId())
            {
                return Forbid();
            }
            var notifications = await this.Repository.GetPendingAPINotificationsAsync(id, Client.SendStatusUpdateNotificationAsync);
            return Ok(notifications);
        }

        [HttpGet]
        [Route("Pending")]
        public async Task<ActionResult<IEnumerable<NotificationViewModel>>> PendingNotifications()
        {
            var organisationId = HttpContext.GetOrganisationId();
            var notifications = await this.Repository.GetPendingAPINotificationsAsync(organisationId.Value, Client.SendStatusUpdateNotificationAsync);
            return Ok(notifications);
        }


        [HttpGet]
        [Route("{id}/{timeStamp}")]
        public async Task<ActionResult<IEnumerable<NotificationViewModel>>> GetAllNotificationsFromTimeStamp(Guid id, DateTime timeStamp)
        {
            if (id != HttpContext.GetOrganisationId())
            {
                return Forbid();
            }
            var notifications = await this.Repository.GetAllNotificationsFromTimeStampAsync(id, timeStamp, Client.SendStatusUpdateNotificationAsync);
            return Ok(notifications);
        }

        [HttpGet]
        [Route("{timeStamp}")]
        public async Task<ActionResult<IEnumerable<NotificationViewModel>>> GetAllNotificationsFromTimeStamp(DateTime timeStamp)
        {
            var organisationId = HttpContext.GetOrganisationId();
            var notifications = await this.Repository.GetAllNotificationsFromTimeStampAsync(organisationId.Value, timeStamp, Client.SendStatusUpdateNotificationAsync);
            return Ok(notifications);
        }


        [HttpGet]
        [Route("{id}/ResetData")]
        public async Task<ActionResult<ResetDataResult>> ResetData(Guid id)
        {
            if (id != HttpContext.GetOrganisationId())
            {
                return Forbid();
            }
            if (!IsResetEnabled)
            {
                return NoContent();
            }
            await this.Repository.RefreshClientData(id);
            return Ok(new ResetDataResult {  Notifications = "reset" });
        }

        [HttpGet]
        [Route("ResetData")]
        public async Task<ActionResult<ResetDataResult>> ResetData()
        {
            var organisationId = HttpContext.GetOrganisationId();
            if (!IsResetEnabled)
            {
                return NoContent();
            }
            await this.Repository.RefreshClientData(organisationId.Value);
            return Ok(new ResetDataResult { Notifications = "reset" });
        }       
    }
}