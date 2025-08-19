using Core.Application.Common.Interfaces;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace InventoryManagement.API.Validation.Common
{
   public sealed class MaxLengthProvider : IMaxLengthProvider
    {
        private readonly IModel _model;

        public MaxLengthProvider(ApplicationDbContext dbContext)
        {
            _model = dbContext.Model;
        }

        public int? GetMaxLength<T>(string propertyName) where T : class
        {
            var entityType = _model.FindEntityType(typeof(T))
                ?? throw new InvalidOperationException($"Entity type {typeof(T).Name} not found in the model.");

            var property = entityType.FindProperty(propertyName)
                ?? throw new InvalidOperationException($"Property {propertyName} not found on {typeof(T).Name}.");

            // 1) Prefer EF’s configured MaxLength (HasMaxLength)
            var maxLen = property.GetMaxLength();
            if (maxLen.HasValue) return maxLen;

            // 2) Fallback to the configured column type, e.g., nvarchar(100), varchar(50), char(10), nchar(20)
            var columnType = property.GetColumnType(); // reliable in EF Core
            if (string.IsNullOrWhiteSpace(columnType)) return null;

            if (TryParseLength(columnType, out var parsed))
                return parsed;

            return null;
        }

        private static bool TryParseLength(string columnType, out int? length)
        {
            length = null;
            var t = columnType.Trim().ToLowerInvariant();

            // varchar(max) / nvarchar(max) ⇒ no fixed limit
            if (t.Contains("(max)")) return true;

            var open = t.IndexOf('(');
            var close = t.IndexOf(')');
            if (open > 0 && close > open + 1)
            {
                var inner = t.Substring(open + 1, close - open - 1).Trim();
                if (int.TryParse(inner, out var n))
                {
                    length = n;
                    return true;
                }
            }
            return false;
        }
    }
}
