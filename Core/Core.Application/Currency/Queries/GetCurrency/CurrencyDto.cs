using Core.Application.Common.HttpResponse;
using Core.Application.Common.Mappings;
using Core.Domain.Enums;

namespace Core.Application.Currency.Queries.GetCurrency
{
    public class CurrencyDto :IMapFrom<ApiResponseDTO <Core.Domain.Entities.Currency>>
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public CurrencyEnum.CurrencyStatus IsActive { get; set; }
        public CurrencyEnum.CurrencyDelete IsDeleted { get; set; }
    }
}