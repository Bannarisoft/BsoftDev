using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using MediatR;


namespace Core.Application.Units.Queries.GetUnits
{
    public class GetUnitQuery : IRequest<ApiResponseDTO<List<GetUnitsDTO>>>;  
  
}