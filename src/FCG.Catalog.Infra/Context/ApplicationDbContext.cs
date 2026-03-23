using FCG.Catalog.Domain.Common;
using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Mediatr;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json;

namespace FCG.Catalog.Infra.Context
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IMediatorHandler _mediatorHandler;
        public DbSet<StoredEvent> StoredEvents { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IMediatorHandler mediatorHandler)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
            _mediatorHandler = mediatorHandler;
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<string>()
                .HaveColumnType("varchar(MAX)");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<ValidationResult>();
            modelBuilder.Ignore<Event>();

            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            ApplySoftDeleteQueryFilter(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes()
                .Where(t => typeof(EntityBase).IsAssignableFrom(t.ClrType));

            foreach (var entityType in entityTypes)
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var isDeletedProperty = Expression.Property(parameter, nameof(EntityBase.IsDeleted));
                var compare = Expression.Equal(isDeletedProperty, Expression.Constant(false));
                var lambda = Expression.Lambda(compare, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();

            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("CreatedAt") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("CreatedAt").IsModified = false;
                    entry.Property("UpdatedAt").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Deleted)
                {
                    foreach (var property in entry.Properties)
                    {
                        property.IsModified = false;
                    }

                    entry.Property("IsDeleted").IsModified = true;
                    entry.Property("DeletedAt").IsModified = true;
                    entry.Property("DeletedAt").CurrentValue = DateTime.Now;
                    entry.State = EntityState.Modified;
                }
            }

            var domainEntities = ChangeTracker.Entries<EntityBase>()
                .Where(x => x.Entity.Notifications != null && x.Entity.Notifications.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Notifications)
                .ToList();

            foreach (var entry in domainEntities)
                foreach (var domainEvent in entry.Entity.Notifications)
                    StoredEvents.Add(new StoredEvent(
                        entry.Entity.Id,
                        entry.Entity.GetType().Name,
                        domainEvent.GetType().Name,
                        JsonSerializer.Serialize((object)domainEvent)
                    ));

            domainEntities.ForEach(e => e.Entity.ClearEvents());

            var affectedRows = await base.SaveChangesAsync(cancellationToken);

            if (domainEvents.Count > 0)
            {
                var tasks = domainEvents.Select(e => _mediatorHandler.PublishEvent(e));
                await Task.WhenAll(tasks);
            }

            return affectedRows;
        }
    }

    public static class MediatorExtension
    {
        public static async Task PublishEvents<T>(this IMediatorHandler mediator, T ctx) where T : DbContext
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<EntityBase>()
                .Where(x => x.Entity.Notifications != null && x.Entity.Notifications.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Notifications)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearEvents());

            var tasks = domainEvents
                .Select(async (domainEvent) =>
                {
                    await mediator.PublishEvent(domainEvent);
                });

            await Task.WhenAll(tasks);
        }
    }

}
