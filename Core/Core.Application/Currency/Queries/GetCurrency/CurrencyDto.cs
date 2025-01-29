using Core.Application.Common.HttpResponse;
using Core.Application.Common.Mappings;

namespace Core.Application.Currency.Queries.GetCurrency
{
    public class CurrencyDto :IMapFrom<ApiResponseDTO <Core.Domain.Entities.Currency>>
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public byte IsActive { get; set; }
    }
}