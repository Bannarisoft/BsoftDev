using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepreciationDetail;
using Core.Application.DepreciationDetail.Queries.GetDepreciationDetail;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.DepreciationDetail.Commands.CreateDepreciationDetail
{
    public class CreateDepreciationDetailCommandHandler : IRequestHandler<CreateDepreciationDetailCommand, ApiResponseDTO<string>>
    {
        private readonly IMapper _mapper;
        private readonly IDepreciationDetailQueryRepository _depreciationDetailQueryRepository;
        private readonly IMediator _mediator;

        public CreateDepreciationDetailCommandHandler(
            IMapper mapper,
            IDepreciationDetailQueryRepository depreciationDetailQueryRepository,
            IMediator mediator)
        {
            _mapper = mapper;
            _depreciationDetailQueryRepository = depreciationDetailQueryRepository;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<string>> Handle(CreateDepreciationDetailCommand request, CancellationToken cancellationToken)
        {
            // Check if Depreciation already exists
          /*   var exists = await _depreciationDetailQueryRepository.ExistDataAsync(
                request.companyId, request.unitId, request.finYearId, request.depreciationType, request.depreciationPeriod
            );

            if (exists)
            {
                return new ApiResponseDTO<string>
                {
                    IsSuccess = false,
                    Message = "Depreciation details already exist for the given parameters."
                };
            }
 */
            // Call CreateAsync and get the status message and code
            var (creationMessage, statusCode) = await _depreciationDetailQueryRepository.CreateAsync(               
                request.finYearId,
                request.depreciationType,
                request.depreciationPeriod
            );

            // Handle the response based on status code
            if (statusCode == 1)
            {
                // Success
                return new ApiResponseDTO<string>
                {
                    IsSuccess = true,
                    Message = creationMessage
                };
            }
            else if (statusCode == -1)
            {
                // Error/Warning
                return new ApiResponseDTO<string>
                {
                    IsSuccess = false,
                    Message = creationMessage
                };
            }
            else
            {
                // Default unknown error
                return new ApiResponseDTO<string>
                {
                    IsSuccess = false,
                    Message = "Unknown error occurred."
                };
            }
        }
    }

}
