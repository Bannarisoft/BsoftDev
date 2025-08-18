using System.Globalization;
using System.Reflection;
using Core.Application.Common.Interfaces;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public abstract class ItemLogRepositoryBase
{
    protected readonly ApplicationDbContext _db;
    private readonly IIPAddressService _ipAddressService;

    public record PropertyChange(string Property, string? OldValue, string? NewValue);

    // Properties we don't want to log (keys/audit/rowversion etc.)
    private static readonly HashSet<string> IgnoredProps = new(StringComparer.OrdinalIgnoreCase)
    {
        "Id", "ItemId",
        "CreatedBy", "CreatedByName", "CreatedDate", "CreatedIP",
        "ModifiedBy", "ModifiedByName", "ModifiedDate", "ModifiedIP",
        "RowVersion", "Timestamp"
    };
   
    protected ItemLogRepositoryBase(ApplicationDbContext db, IIPAddressService ipAddressService)
    {
        _db = db;_ipAddressService = ipAddressService;
    }

    // ---------- value formatting helpers ----------
    private static string? ToValueString(object? value)
    {
        if (value is null) return null;

        switch (value)
        {
            case DateTime dt:
                return dt.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz", CultureInfo.InvariantCulture);
            case DateTimeOffset dto:
                return dto.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz", CultureInfo.InvariantCulture);
            case decimal dec:
                return dec.ToString(CultureInfo.InvariantCulture);
            case double d:
                return d.ToString(CultureInfo.InvariantCulture);
            case float f:
                return f.ToString(CultureInfo.InvariantCulture);
            case bool b:
                return b ? "true" : "false";
            case Enum e:
                return e.ToString(); // by name
            default:
                return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }

    private static bool IsIgnored(string propName) => IgnoredProps.Contains(propName);

    // ---------- diff helpers ----------
    protected List<PropertyChange> GetModifiedProps(EntityEntry entry)
    {
        var changes = new List<PropertyChange>();

        foreach (var p in entry.Properties)
        {
            // Only consider props EF marked as modified and that we don't ignore
            if (!p.IsModified) continue;
            if (IsIgnored(p.Metadata.Name)) continue;

            var oldVal = ToValueString(p.OriginalValue);
            var newVal = ToValueString(p.CurrentValue);

            if (!string.Equals(oldVal, newVal, StringComparison.Ordinal))
                changes.Add(new PropertyChange(p.Metadata.Name, oldVal, newVal));
        }

        return changes;
    }

    /// <summary>
    /// Use this when you had to attach/replace a detached entity (AsNoTracking retrieval).
    /// </summary>
    protected List<PropertyChange> DiffByReflection<T>(T original, T updated)
    {
        var changes = new List<PropertyChange>();
        if (original is null || updated is null) return changes;

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var pi in props)
        {
            if (!pi.CanRead) continue;
            if (IsIgnored(pi.Name)) continue;

            var ov = ToValueString(pi.GetValue(original));
            var nv = ToValueString(pi.GetValue(updated));

            if (!string.Equals(ov, nv, StringComparison.Ordinal))
                changes.Add(new PropertyChange(pi.Name, ov, nv));
        }

        return changes;
    }

    // ---------- logging helpers ----------
    /// <summary>
    /// Adds Update logs only if there are real changes (returns true if log entries were added).
    /// DO NOT call SaveChanges here; let the UoW handle it.
    /// </summary>
    protected bool TryAddUpdateLog(string entityName, int entityId, IEnumerable<PropertyChange> changes)
    {
        if (changes is null) return false;

        // Filter out any accidentals where old==new
        var material = changes
            .Where(c => !string.Equals(c.OldValue, c.NewValue, StringComparison.Ordinal))
            .ToList();

        if (material.Count == 0)
            return false;

        foreach (var c in material)
        {
            _db.ItemLog.Add(new Core.Domain.Entities.Item.ItemDetail.ItemLog
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = "Update",
                PropertyName = c.Property,
                OldValue = c.OldValue,
                NewValue = c.NewValue,
                CreatedBy = int.Parse(_ipAddressService.GetCurrentUserId()),
                CreatedDate = DateTime.UtcNow,
                CreatedByName = _ipAddressService.GetUserName(),
                CreatedIP = _ipAddressService.GetSystemIPAddress()
            });
        }

        return true;
    }

    /* protected void AddInsertLog(string entityName, int entityId)
    {
        _db.ItemLog.Add(new Core.Domain.Entities.Item.ItemDetail.ItemLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = "Insert",
            PropertyName = "*",
            OldValue = null,
            NewValue = null
        });
    } */
}
