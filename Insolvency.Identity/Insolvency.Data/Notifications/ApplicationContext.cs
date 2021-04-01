using Insolvency.Notifications.Models;
using Microsoft.EntityFrameworkCore;

namespace Insolvency.Data.Notifications
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<NotificationMessage> Messages { get; set; }
        public DbSet<Sender> Senders { get; set; }
        public DbSet<NotificationOwner> Owners { get; set; }
    }
}
