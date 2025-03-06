
using Core.Application.Common.HttpResponse;
using Core.Application.DepreciationDetail.Queries.GetDepreciationDetail;
using MediatR;

namespace Core.Application.DepreciationDetail.Commands.DeleteDepreciationDetail
{
    public class DeleteDepreciationDetailCommand :  IRequest<ApiResponseDTO<DepreciationDto>>  
    {
         public int Id { get; set; }        
    }
}