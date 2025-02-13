using Core.Application.Common.HttpResponse;
using Core.Application.Manufacture.Queries.GetManufacture;
using MediatR;

namespace Core.Application.Manufacture.Queries.GetManufactureAutoComplete
{
    public class GetManufactureAutoCompleteQuery : IRequest<ApiResponseDTO<List<ManufactureAutoCompleteDTO>>>
    {
         public string? SearchPattern { get; set; }
    }
}