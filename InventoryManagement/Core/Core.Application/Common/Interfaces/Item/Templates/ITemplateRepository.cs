using Core.Domain.Entities.Item.ItemDetail.Templates;

namespace Core.Application.Common.Interfaces.Item.Templates
{
    public interface ITemplateRepository
    {
        Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
        Task<int> CreateAsync(InspectionTemplate template, CancellationToken ct = default);
        Task<InspectionTemplate?> GetAsync(int id, CancellationToken ct = default);
        Task<List<InspectionTemplate>> SearchAsync(string? term, int take, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
