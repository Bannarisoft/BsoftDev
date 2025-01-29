using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IFinancialYear;
using Core.Application.FinancialYear.Queries.GetFinancialYear;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.FinancialYear.Queries.GetFinancialYearGetById
{
    public class GetFinancialYearByIdQueryHanlder  :IRequestHandler<GetFinancialYearByIdQuery,ApiResponseDTO<List<FinancialYearDto>>>
    {
             
        private readonly IFinancialYearQueryRepository _financialyearRepository;
        private readonly IMapper _mapper;
         private readonly IMediator _mediator;
           private readonly ILogger<GetFinancialYearByIdQueryHanlder> _logger;


        public GetFinancialYearByIdQueryHanlder(IFinancialYearQueryRepository financialyearRepository,IMapper mapper , IMediator mediator, ILogger<GetFinancialYearByIdQueryHanlder> logger)    
        {
            _financialyearRepository = financialyearRepository;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ApiResponseDTO<List<FinancialYearDto>>> Handle(GetFinancialYearByIdQuery request, CancellationToken cancellationToken)
        {
          
          _logger.LogInformation("Fetching FinancialYear Request started: {Request}", request);

                    // Fetch department by ID
                    var financialyear = await _financialyearRepository.GetByIdAsync(request.FYId);
                    
                    if (financialyear == null)
                    {
                        _logger.LogWarning("FinancialYear with ID {DepartmentId} not found.", request.FYId);

                        return new ApiResponseDTO<List<FinancialYearDto>>{ IsSuccess = false,Message = "FinancialYear not found.", Data = null };
                    }
            

              var financialyearDto = _mapper.Map<FinancialYearDto>(financialyear);
 //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: financialyearDto.Id.ToString(),        
                    actionName: financialyearDto.StartYear,                
                    details: $"FinancialYear '{financialyearDto.StartYear}' was created. FinancialYearCode: {financialyearDto.Id}",
                    module:"FinancialYear"
                );

                await _mediator.Publish(domainEvent, cancellationToken);
           // return new ApiResponseDTO<FinancialYearDto> { IsSuccess = true, Message = "Success", Data = dfinancialyearDto };
           return new ApiResponseDTO<List<FinancialYearDto>> { IsSuccess = true, Message = "Success", Data = new List<FinancialYearDto> { financialyearDto }};
        }
    }
}