using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IActivityCheckListMaster;
using Core.Domain.Events;
using MassTransit;
using MediatR;

namespace Core.Application.ActivityCheckListMaster.Queries.GetActivityCheckListMaster
{
    public class GetAllActivityCheckListMasterQueryHandler : IRequestHandler<GetAllActivityCheckListMasterQuery, ApiResponseDTO<List<GetAllActivityCheckListMasterDto>>>
    {


         private readonly IActivityCheckListMasterQueryRepository _activityCheckListMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetAllActivityCheckListMasterQueryHandler(IActivityCheckListMasterQueryRepository activityCheckListMasterQueryRepository, IMapper mapper, IMediator mediator)        
        {
            _activityCheckListMasterQueryRepository = activityCheckListMasterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }



        public async Task<ApiResponseDTO<List<GetAllActivityCheckListMasterDto>>> Handle(GetAllActivityCheckListMasterQuery request, CancellationToken cancellationToken)
        {
            // Fetch data from repository
            var (checkLists, totalCount) = await _activityCheckListMasterQueryRepository.GetAllActivityCheckListMasterAsync(request.PageNumber, request.PageSize, request.SearchTerm);

            // Map domain entities to DTOs
            var checkListDto = _mapper.Map<List<GetAllActivityCheckListMasterDto>>(checkLists);

            // Publish domain event for auditing
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",
                actionName: "",
                details: "Activity Checklist Master details were fetched.",
                module: "ActivityCheckListMaster"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            // Return API response
            return new ApiResponseDTO<List<GetAllActivityCheckListMasterDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = checkListDto,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}