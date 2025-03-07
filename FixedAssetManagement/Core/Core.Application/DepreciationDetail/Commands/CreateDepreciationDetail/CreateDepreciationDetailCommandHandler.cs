using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepreciationDetail;
using Core.Application.DepreciationDetail.Queries.GetDepreciationDetail;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.DepreciationDetail.Commands.CreateDepreciationDetail
{
    public class CreateDepreciationDetailCommandHandler : IRequestHandler<CreateDepreciationDetailCommand, ApiResponseDTO<DepreciationDto>>
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

     public async Task<ApiResponseDTO<DepreciationDto>> Handle(CreateDepreciationDetailCommand request, CancellationToken cancellationToken)
        {
            // Check if Depreciation already exists
            var exists = await _depreciationDetailQueryRepository.ExistDataAsync(
                request.companyId, request.unitId, request.finYear, request.startDate, request.endDate,request.depreciationType);

            if (exists)
            {
                return new ApiResponseDTO<DepreciationDto>
                {
                    IsSuccess = false,
                    Message = "Depreciation details already exist for the given parameters."
                };
            }

            // Insert into database using stored procedure or EF Core
            var result = await _depreciationDetailQueryRepository.CreateAsync(
                request.companyId,
                request.unitId,
                request.finYear,
                request.startDate,request.endDate,request.depreciationType
            );

            // Domain Event for Audit Logging
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: request.companyId.ToString(),
                actionName: request.finYear ?? string.Empty,
                details: $"Depreciation detail for Finyear {request.finYear} was created.",
                module: "Depreciation"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            // ✅ FIX: Map to `DepreciationDto`, NOT `DepreciationCalculationDto`
            var depreciationDTO = _mapper.Map<DepreciationDto>(result);

            if (depreciationDTO != null)
            {
                return new ApiResponseDTO<DepreciationDto>
                {
                    IsSuccess = true,
                    Message = "Depreciation details created successfully.",
                    Data = depreciationDTO
                };
            }

            return new ApiResponseDTO<DepreciationDto>
            {
                IsSuccess = false,
                Message = "Depreciation details could not be created."
            };
        }

    }
}
