using Core.Application.Common.Mappings;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Core.Application.Units.Queries.GetUnits
{
    public class GetUnitQuery : IRequest<List<UnitDto>>;  
  
}