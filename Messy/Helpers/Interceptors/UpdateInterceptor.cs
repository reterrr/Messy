using Messy.Helpers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Messy.Helpers.Interceptors;

public class UpdateInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        
        IEnumerable<EntityEntry<IUpdatable>> entities = 
            eventData
                .Context
                .ChangeTracker
                .Entries<IUpdatable>()
                .Where(e => e.State == EntityState.Modified);
        
        foreach (EntityEntry<IUpdatable> updatables in entities)
        {
            updatables.State = EntityState.Modified;
            updatables.Entity.UpdatedAt = DateTime.UtcNow;
        }
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}