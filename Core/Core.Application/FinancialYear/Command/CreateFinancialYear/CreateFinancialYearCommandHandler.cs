using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IFinancialYear;
using Core.Application.FinancialYear.Queries.GetFinancialYear;
using Core.Application.FinancialYear.Command.CreateFinancialYear;
using MediatR;
using Microsoft.Extensions.Logging;
using Core.Domain.Events;


namespace Core.Application.FinancialYear.Command.CreateFinancialYear
{
    public class CreateFinancialYearCommandHandler :IRequestHandler<CreateFinancialYearCommand, ApiResponseDTO<Core.Domain.Entities.FinancialYear>>
    {


     private readonly IFinancialYearCommandRepository  _financialYearCommandRepository;
        private readonly IMapper _mapper;
          private readonly IMediator _mediator; 
          private readonly ILogger<CreateFinancialYearCommandHandler> _logger;

           public CreateFinancialYearCommandHandler(IFinancialYearCommandRepository financialYearCommandRepository,IMapper mapper, IMediator mediator ,ILogger<CreateFinancialYearCommandHandler> logger)
        {
             _financialYearCommandRepository=financialYearCommandRepository;
            _mapper=mapper;
            _mediator=mediator;
            _logger=logger;

        }   

        public async Task<ApiResponseDTO<Core.Domain.Entities.FinancialYear>> Handle(CreateFinancialYearCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("Starting CreateDepartmentCommandHandler for request: {@Request}", request);

                        // Map the request to the entity
                        var  financialYearEntity = _mapper.Map<Core.Domain.Entities.FinancialYear>(request);
                        _logger.LogInformation("Mapped Create FinancialYear Command to Department entity: {@DepartmentEntity}", financialYearEntity);

                        // Save the department
                        var createdfinancialYear = await _financialYearCommandRepository.CreateAsync(financialYearEntity);

                        if (createdfinancialYear == null)
                        {
                            _logger.LogWarning("Failed to create department. FinancialYear entity: {@financialYearEntity}", financialYearEntity);
                            return new ApiResponseDTO<Core.Domain.Entities.FinancialYear> {IsSuccess = false, Message = "FinancialYear not created" };
                        }

                        _logger.LogInformation("FinancialYear successfully created with ID: {FYId}", createdfinancialYear.Id);

                        // Publish the domain event
                        var domainEvent = new AuditLogsDomainEvent(
                            actionDetail: "Create",
                            actionCode: createdfinancialYear.Id.ToString(),
                            actionName: createdfinancialYear.StartYear,
                            details: $"FinancialYear '{createdfinancialYear.StartYear}' was created. FinancialYearID: {createdfinancialYear.Id}",
                            module: "FinancialYear"
                        );

                        await _mediator.Publish(domainEvent, cancellationToken);
                        _logger.LogInformation("AuditLogsDomainEvent published for FinancialYear ID: {FYId}", createdfinancialYear.Id);

                        // Map the result to DTO
                        var deptDto = _mapper.Map<FinancialYearDto>(createdfinancialYear);

                        _logger.LogInformation("Returning success response for FinancialYear ID: {FYId}", createdfinancialYear.Id);

                        return new ApiResponseDTO<Core.Domain.Entities.FinancialYear>
                        {
                            IsSuccess = true,
                            Message = "FinancialYear created successfully"
                            
                        };


        }

        
    }
}