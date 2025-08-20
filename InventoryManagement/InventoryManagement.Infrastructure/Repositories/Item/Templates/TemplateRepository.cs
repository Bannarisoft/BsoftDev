using Core.Application.Common.Interfaces.Item.Templates;
using Core.Domain.Entities.Item.ItemDetail.Templates;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories.Item.Templates
{
    public sealed class TemplateRepository : ITemplateRepository
    {
        private readonly ApplicationDbContext _db;
        public TemplateRepository(ApplicationDbContext db) => _db = db;

        public Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default) =>
            _db.InspectionTemplate.AnyAsync(t =>
                t.IsDeleted == Core.Domain.Common.BaseEntity.IsDelete.NotDeleted &&
                t.TemplateName == name, ct);

        public async Task<int> CreateAsync(InspectionTemplate template, CancellationToken ct = default)
        {
            _db.InspectionTemplate.Add(template);
            await _db.SaveChangesAsync(ct);
            return template.Id;
        }

        public Task<InspectionTemplate?> GetAsync(int id, CancellationToken ct = default) =>
            _db.InspectionTemplate
               .Include(t => t.Parameters)
               .FirstOrDefaultAsync(t => t.Id == id && t.IsDeleted == Core.Domain.Common.BaseEntity.IsDelete.NotDeleted, ct);

        public async Task<List<InspectionTemplate>> SearchAsync(string? term, int take, CancellationToken ct = default)
        {
            var q = _db.InspectionTemplate.AsNoTracking()
                .Where(t => t.IsDeleted == Core.Domain.Common.BaseEntity.IsDelete.NotDeleted);

            if (!string.IsNullOrWhiteSpace(term))
                q = q.Where(t => t.TemplateName.Contains(term));

            return await q
                .OrderBy(t => t.TemplateName)
                .Take(take <= 0 ? 20 : take)
                .Include(t => t.Parameters)
                .ToListAsync(ct);
        }
         public Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
            _db.InspectionTemplate
               .AnyAsync(t => t.Id == id
                           && t.IsDeleted == Core.Domain.Common.BaseEntity.IsDelete.NotDeleted, ct);
    }
}
