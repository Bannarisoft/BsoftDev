using Core.Application.Common;
using Core.Application.Units.Queries.GetUnits;
using MediatR;

namespace Core.Application.Units.Queries.GetUnitById
{
    public class GetUnitByIdQuery :  IRequest<Result<List<UnitDto>>>
    { 
        public int Id { get; set; }
    }
    
}