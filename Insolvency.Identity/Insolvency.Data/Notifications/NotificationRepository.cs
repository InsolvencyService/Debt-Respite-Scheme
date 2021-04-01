using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insolvency.Interfaces.Notifications;
using Insolvency.Notifications.Models;
using Microsoft.EntityFrameworkCore;

namespace Insolvency.Data.Notifications
{
    public class NotificationRepository : INotificationRepository
    {
        public ApplicationContext Context { get; }

        public NotificationRepository(ApplicationContext context)
        {
            this.Context = context;
        }

        public async Task StoreAsync(NotificationMessage message, Func<Task<string>> inTransactionExecutionAsync)
        {
            using (var transaction = this.Context.Database.BeginTransaction())
            {
                await this.UpdateMessageOwner(message);
                message.SenderSystem = await this.Context.Senders.FirstOrDefaultAsync(x => x.Id == message.SenderSystem.Id);
                message.Status = NotificationStatus.Sent;
                this.Context.Messages.Add(message);
                var notifyResultId = await inTransactionExecutionAsync();
                message.ExternalId = notifyResultId;
                await this.Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }

        public async Task UpdateStatusForSentMessages(
            Func<Guid, IEnumerable<NotificationMessage>,Task<StatusUpdateMessage>> checkStatusAsync,
            Func<IEnumerable<StatusUpdateMessage>, Task> notifyChangesAsync)
        {
            using (var transaction = this.Context.Database.BeginTransaction())
            {
                var result = await this.Context
                    .Messages
                    .Include(x => x.SenderSystem)
                    .Where(x => x.Type != NotificationType.API)
                    .Where(x => x.Status == NotificationStatus.Sent)
                    .Take(500)
                    .ToListAsync();
                var groupedResult = result.GroupBy(x => x.SenderSystem.Id);
                var updates = new List<StatusUpdateMessage>();
                foreach (var senderSystemNotifications in groupedResult)
                {
                    var update = await checkStatusAsync(senderSystemNotifications.Key, senderSystemNotifications);
                    updates.Add(update);
                }
                await notifyChangesAsync(updates);
                await this.Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }

        public async Task<IEnumerable<NotificationViewModel>> GetPendingAPINotificationsAsync(
            Guid ownerId,
            Func<IEnumerable<StatusUpdateMessage>, Task> notifyChangesAsync)
        {
            using (var transaction = this.Context.Database.BeginTransaction())
            {
                var messages = await this.Context
                    .Messages
                    .Include(x => x.SenderSystem)
                    .Where(x => x.Owner.Id == ownerId)
                    .Where(x => x.Type == NotificationType.API)
                    .Where(x => x.Status == NotificationStatus.Sent)
                    .OrderBy(x => x.CreatedOn)
                    .Take(500)
                    .ToListAsync();
                messages.ForEach(x => x.Status = NotificationStatus.Completed);

                var result = messages.Select(x => new NotificationViewModel(x)).ToList();
                var statusUpdates = MapToStatusUpdates(messages);
                await notifyChangesAsync(statusUpdates);
                await this.Context.SaveChangesAsync();
                await transaction.CommitAsync();
                return result;
            }
        }

        public async Task<IEnumerable<NotificationViewModel>> GetAllNotificationsFromTimeStampAsync(
            Guid ownerId,
            DateTime timeStamp,
            Func<IEnumerable<StatusUpdateMessage>, Task> notifyChangesAsync)
        {
            using (var transaction = this.Context.Database.BeginTransaction())
            {
                var messages = await this.Context
                    .Messages
                    .Include(x => x.SenderSystem)
                    .Where(x => x.Owner.Id == ownerId)
                    .Where(x => x.Type == NotificationType.API)
                    .Where(x => x.CreatedOn > timeStamp)
                    .OrderBy(x => x.CreatedOn)
                    .Take(1000)
                    .ToListAsync();

                var pendingMessages = messages
                    .Where(x => x.Status == NotificationStatus.Sent)
                    .ToList();
                pendingMessages.ForEach(x => x.Status = NotificationStatus.Completed);

                var result = messages.Select(x => new NotificationViewModel(x)).ToList();
                var statusUpdates = MapToStatusUpdates(pendingMessages);
                await notifyChangesAsync(statusUpdates);
                await this.Context.SaveChangesAsync();
                await transaction.CommitAsync();
                return result;
            }
        }

        public async Task<int> RefreshClientData(Guid ownerId)
        {
            var result = await this.Context.Database.ExecuteSqlInterpolatedAsync($"Update public.\"Messages\" Set \"Status\" = 1 where \"OwnerId\" = {ownerId}");

            return result;
        }

        public async Task NotifyOwnersForPendingNotificationsAsync(Func<IEnumerable<NotificationOwner>, Task> notifyChangesAsync)
        {
            var owners = await this.Context
                .Messages
                .Include(x => x.Owner)
                .Where(x => x.Type == NotificationType.API)
                .Where(x => x.Status == NotificationStatus.Sent)
                .Select(x => x.Owner)
                .Distinct()
                .ToListAsync();

            await notifyChangesAsync(owners);
        }

        protected virtual async Task UpdateMessageOwner(NotificationMessage message)
        {
            if (message.Owner == null)
            {
                return;
            }
            var dbOwner = await this.Context.Owners.FirstOrDefaultAsync(x => x.Id == message.Owner.Id);
            if (dbOwner == null)
            {
                return;
            }
            dbOwner.Email = message.Owner.Email;
            dbOwner.TemplateId = message.Owner.TemplateId;
            message.Owner = dbOwner;
        }

        protected virtual IEnumerable<StatusUpdateMessage> MapToStatusUpdates(IEnumerable<NotificationMessage> messages)
        {
            var result =  messages
                .GroupBy(x => x.SenderSystem.Id)
                .Select(x => new StatusUpdateMessage
                {
                    SenderSystemId = x.Key,
                    Updates = x.Select(
                        y => new StatusUpdateNotification
                        {
                            Id = y.ExternalSenderSystemId,
                            Status = y.Status,
                            Type = y.Type
                        }
                    )
                })
                .ToList();
            return result;
        }
    }
}
