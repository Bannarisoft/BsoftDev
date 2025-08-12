using System.Reflection;
using System.Text.Json;
using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Domain.Entities.Item.ItemDetail;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public abstract class ItemLogRepositoryBase
{
    protected readonly ApplicationDbContext _db;
    protected readonly IExecutionContext _ctx;

    protected ItemLogRepositoryBase(ApplicationDbContext db, IExecutionContext ctx)
    {
        _db = db; _ctx = ctx;
    }

    // properties we never log (navs, audit columns, keys you don't change)
    private static readonly HashSet<string> IgnoreProps = new(StringComparer.OrdinalIgnoreCase)
    {
        "Id","ItemId",
        "Item","ParentItem","ChildItems","VariantValues","VariantDefs",
        "CreatedBy","CreatedByName","CreatedIP","CreatedDate",
        "ModifiedBy","ModifiedByName","ModifiedIP","ModifiedDate",
        "IsDeleted"
    };

    protected sealed record PropertyChange(string Property, object? Old, object? New);

    protected List<PropertyChange> GetModifiedProps<TEntity>(EntityEntry<TEntity> entry)
        where TEntity : class
    {
        var list = new List<PropertyChange>();
        foreach (var p in entry.Properties)
        {
            if (!p.IsModified || IgnoreProps.Contains(p.Metadata.Name)) continue;
            var oldVal = p.OriginalValue;
            var newVal = p.CurrentValue;
            if (Equals(oldVal, newVal)) continue;
            list.Add(new PropertyChange(p.Metadata.Name, oldVal, newVal));
        }
        return list;
    }

    // Detached case: compute diffs by reflection (original vs updated)
    protected static List<PropertyChange> DiffByReflection<TEntity>(TEntity original, TEntity updated)
    {
        var list = new List<PropertyChange>();
        var props = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite && !IgnoreProps.Contains(p.Name));
        foreach (var pi in props)
        {
            var oldVal = pi.GetValue(original);
            var newVal = pi.GetValue(updated);
            if (!Equals(oldVal, newVal))
                list.Add(new PropertyChange(pi.Name, oldVal, newVal));
        }
        return list;
    }

    protected void AddUpdateLog(string entityName, int entityId, List<PropertyChange> changes)
    {
        if (changes.Count == 0) return;
        var log = new ItemLog
        {
            CreatedDate = DateTimeOffset.UtcNow,
            EntityName = entityName,
            EntityId = entityId,
            Action = "Update",
            ChangesJson = JsonSerializer.Serialize(changes, new JsonSerializerOptions(JsonSerializerDefaults.Web)),
            CreatedBy = _ctx.CreatedBy,
            CreatedByName = _ctx.CreatedByName,
            CreatedIP = _ctx.CreatedIP,
            CorrelationId = _ctx.CorrelationId
        };
        _db.ItemLog.Add(log);
    }
}
