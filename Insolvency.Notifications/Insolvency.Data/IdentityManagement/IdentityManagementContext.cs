using System.Threading;
using System.Threading.Tasks;

using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;

using Insolvency.Identity.Models;
using Insolvency.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Insolvency.Data.IdentityManagement
{
    public class IdentityManagementContext : ConfigurationDbContext<IdentityManagementContext>
    {
        private IChangeTrackerAuditing ChangeTrackerAuditing { get; set; }

        public IdentityManagementContext(
            DbContextOptions<IdentityManagementContext> options,
            ConfigurationStoreOptions storeOptions,
            IChangeTrackerAuditing changeTrackerAuditing)
            : base(options, storeOptions)
        {
            ChangeTrackerAuditing = changeTrackerAuditing;
        }

        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<OrganisationScope> OrganisationScopes { get; set; }
        public DbSet<RoleUser> RoleUsers { get; set; }
        public DbSet<RoleUsersOrganisationScope> RoleUsersOrganisationScopes { get; set; }
        public DbSet<RoleUserAssignedClient> RoleUserAssignedClients { get; set; }
        public DbSet<OrganisationRoleUserAssignedClient> OrganisationRoleUserAssignedClients { get; set; }
        public DbSet<OrganisationScopeRoleUserAssignedClient> OrganisationScopeRoleUserAssignedClients { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await ChangeTrackerAuditing.SaveWithAuditTrackingAsync(ChangeTracker, base.SaveChangesAsync, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organisation>(org =>
            {
                org.HasKey(o => o.Id);
                org.Property(o => o.Name).IsRequired();
                org.Property(o => o.Type).IsRequired();
                org.Property(o => o.OnboarderEmail).IsRequired();
                org.HasMany<OrganisationScope>(o => o.Scopes).WithOne(o => o.Organisation).HasForeignKey(o => o.OrganisationId);
                org.HasMany<RoleUser>(o => o.RoleUsers).WithOne(o => o.Organisation).HasForeignKey(o => o.OrganisationId);
            });

            modelBuilder.Entity<RoleUser>(user =>
            {
                user.HasKey(o => o.Id);
                user.Property(o => o.Email).IsRequired();
                user.Property(o => o.NormalisedEmail).IsRequired();
                user.Property(o => o.Role).IsRequired();
                user.Property(o => o.OrganisationId).IsRequired();
                user.HasMany(o => o.Scopes)
                                    .WithOne(o => o.RoleUser)
                                    .HasForeignKey(o => o.RoleUserId);
            });

            modelBuilder.Entity<RoleUsersOrganisationScope>(scope =>
            {
                scope.HasKey(x => x.Id);
                scope.Property(x => x.OrganisationScopeName).IsRequired();
                scope.Property(x => x.RoleUserId);
                scope.HasOne(x => x.RoleUser)
                                .WithMany(x => x.Scopes)
                                .HasForeignKey(x => x.RoleUserId);
            });

            modelBuilder.Entity<OrganisationRoleUserAssignedClient>()
            .HasKey(x => new { x.OrganisationId, x.RoleUserAssignedClientId });

            modelBuilder.Entity<OrganisationRoleUserAssignedClient>(x =>
            {
                x.HasOne(y => y.Organisation)
                .WithMany(y => y.OrganisationRoleUserAssignedClients)
                .HasForeignKey(y => y.OrganisationId);
            });

            modelBuilder.Entity<OrganisationRoleUserAssignedClient>(x =>
            {
                x.HasOne(y => y.RoleUserAssignedClient)
                .WithMany(y => y.OrganisationRoleUserAssignedClients)
                .HasForeignKey(y => y.RoleUserAssignedClientId);
            });

            modelBuilder.Entity<OrganisationScopeRoleUserAssignedClient>()
                .HasKey(x => new { x.OrganisationScopeId, x.RoleUserAssignedClientId });

            modelBuilder.Entity<OrganisationScopeRoleUserAssignedClient>(x =>
            {
                x.HasOne(y => y.OrganisationScope)
                .WithMany(y => y.OrganisationScopeRoleUserAssignedClients)
                .HasForeignKey(y => y.OrganisationScopeId);
            });

            modelBuilder.Entity<OrganisationScopeRoleUserAssignedClient>(x =>
            {
                x.HasOne(y => y.RoleUserAssignedClient)
                .WithMany(y => y.OrganisationScopeRoleUserAssignedClients)
                .HasForeignKey(y => y.RoleUserAssignedClientId);
            });

            modelBuilder.Entity<RoleUserAssignedClient>(x =>
            {
                x.HasOne(y => y.Client).WithOne().HasForeignKey<RoleUserAssignedClient>(y => y.ClientRecordId);
            });
        }
    }

}