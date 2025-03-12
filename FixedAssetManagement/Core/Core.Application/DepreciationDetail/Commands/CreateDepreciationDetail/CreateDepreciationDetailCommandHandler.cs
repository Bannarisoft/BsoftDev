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
    public class CreateDepreciationDetailCommandHandler : IRequestHandler<CreateDepreciationDetailCommand, ApiResponseDTO<List<DepreciationDto>>>
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

        public async Task<ApiResponseDTO<List<DepreciationDto>>> Handle(CreateDepreciationDetailCommand request, CancellationToken cancellationToken)
        {
            // Check if Depreciation already exists
            var exists = await _depreciationDetailQueryRepository.ExistDataAsync(
                request.companyId, request.unitId, request.finYear??string.Empty,  request.depreciationType,  request.depreciationPeriod
            );

            if (exists)
            {
                return new ApiResponseDTO<List<DepreciationDto>>
                {
                    IsSuccess = false,
                    Message = "Depreciation details already exist for the given parameters."
                };
            }

            // Call CreateAsync and get the string message
            var depreciationList = await _depreciationDetailQueryRepository.CreateAsync(
                request.companyId,
                request.unitId,
                request.finYear,               
                request.depreciationType ,request.depreciationPeriod            
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

            return new ApiResponseDTO<List<DepreciationDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = depreciationList // ✅ Returning stored procedure message
            };
        }


    }
}
