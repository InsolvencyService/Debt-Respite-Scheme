using System;
using System.Linq;
using Insolvency.Notifications.Models;
using Microsoft.EntityFrameworkCore;

namespace Insolvency.Data.Notifications
{
    public static class DbInitialise
    {
        public static void Init(ApplicationContext context)
        {
            context.Database.Migrate();
            Seed(context);
        }

        public static void Seed(ApplicationContext context)
        {
            if (context.Senders.Count() == 0)
            {
                var breathingSpaceSender = new Sender
                {
                    Id = new Guid("F6A2D4B8-B1C6-488B-9BA5-F249565FBC1D"),
                    Name = "Breathing Space Case Management System"
                };
                context.Senders.Add(breathingSpaceSender);
                var batchNotificationSender = new Sender
                {
                    Id = new Guid("D033125A-26DD-4A52-A378-14531CDB4410"),
                    Name = "Batch Notification Sender System"
                };
                context.Senders.Add(batchNotificationSender);
                context.SaveChanges();
            }
        }
    }
}

