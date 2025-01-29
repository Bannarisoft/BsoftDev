using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IFinancialYear;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.FinancialYear.Queries.GetFinancialYear
{
    public class GetFinancialYearQueryHandler  :IRequestHandler<GetFinancialYearQuery,ApiResponseDTO<List<FinancialYearDto>>>
    {
         private readonly IFinancialYearQueryRepository _financialyearRepository;
        private readonly IMapper _mapper; 
         private readonly IMediator _mediator; 

         private readonly ILogger<GetFinancialYearQueryHandler> _logger;
      


    
       public GetFinancialYearQueryHandler(IFinancialYearQueryRepository financialyearRepository,IMapper mapper , IMediator mediator, ILogger<GetFinancialYearQueryHandler> logger)
        {
            _mapper =mapper;
            _financialyearRepository = financialyearRepository; 
            _mediator = mediator;     
            _logger = logger;

        }

        public async Task<ApiResponseDTO<List<FinancialYearDto>>> Handle(GetFinancialYearQuery request, CancellationToken cancellationToken)    
        {
         


             _logger.LogInformation("Fetching FinancialYear Request started: {request}", request);
           
           
             var financialyear = await _financialyearRepository.GetAllFinancialYearAsync();
            
             if (financialyear == null || !financialyear.Any())
            {
               _logger.LogWarning("No FinancialYear records found in the database. Total count: {Count}", financialyear?.Count ?? 0);

                  return new ApiResponseDTO<List<FinancialYearDto>> { IsSuccess = false, Message = "No Record Found" };
            }

             var financialyearList = _mapper.Map<List<FinancialYearDto>>(financialyear);
             var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "",
                    details: $"FinancialYear details was fetched.",
                    module:"FinancialYear"
                );

                  await _mediator.Publish(domainEvent, cancellationToken);
              
            _logger.LogInformation("FinancialYear {department} Listed successfully.", financialyearList.Count);
            return new ApiResponseDTO<List<FinancialYearDto>> { IsSuccess = true, Message = "Success", Data = financialyearList };       
        }
    }

}