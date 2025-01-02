using MediatR;


namespace Core.Application.Units.Queries.GetUnits
{
    public class GetUnitQuery : IRequest<List<UnitDto>>;  
  
}