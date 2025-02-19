using Core.Application.Common.HttpResponse;
using Core.Application.SpecificationMaster.Queries.GetSpecificationMaster;
using MediatR;

namespace Core.Application.SpecificationMaster.Queries.GetSpecificationMasterById
{
    public class GetSpecificationMasterByIdQuery : IRequest<ApiResponseDTO<SpecificationMasterDTO>>
    {
        public int Id { get; set; }
    }
}