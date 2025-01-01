using Core.Application.Units.Queries.GetUnits;
using MediatR;

namespace Core.Application.Units.Queries.GetUnitById
{
    public class GetUnitByIdQuery : IRequest<List<UnitDto>>
    { 
        public int Id { get; set; }
    }
    
}