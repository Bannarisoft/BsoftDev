using Core.Application.Common.HttpResponse;
using Core.Application.Units.Queries.GetUnits;
using MediatR;

namespace Core.Application.Units.Commands.UpdateUnit
{
    public class UpdateUnitCommand : IRequest<ApiResponseDTO<int>>
    {    
    //public int UnitId  { get; set; }
    public UpdateUnitsDto UpdateUnitDto { get; set; }  
    }
}