using Core.Application.Common;
using MediatR;


namespace Core.Application.Units.Queries.GetUnits
{
    public class GetUnitQuery : IRequest<Result<List<UnitDto>>>;  
  
}