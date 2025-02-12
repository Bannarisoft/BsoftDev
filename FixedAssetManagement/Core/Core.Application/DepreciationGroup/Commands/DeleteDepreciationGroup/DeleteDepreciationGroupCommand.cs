using Core.Application.Common.HttpResponse;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using MediatR;

namespace Core.Application.DepreciationGroup.Commands.DeleteDepreciationGroup
{
    public class DeleteDepreciationGroupCommand :  IRequest<ApiResponseDTO<DepreciationGroupDTO>>  
    {
          public int Id { get; set; }         
    }
}