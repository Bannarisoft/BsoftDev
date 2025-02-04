using Core.Application.Common.HttpResponse;
using Core.Application.Units.Queries.GetUnits;
using MediatR;

namespace Core.Application.Units.Commands.DeleteUnit
{
    public class DeleteUnitCommand : IRequest<ApiResponseDTO<int>>
    {
            public int UnitId { get; set; }
    }
 
    }
    
