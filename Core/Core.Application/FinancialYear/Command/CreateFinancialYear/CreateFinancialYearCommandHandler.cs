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
    public class CreateFinancialYearCommandHandler :IRequestHandler<CreateFinancialYearCommand, ApiResponseDTO<int>>
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
public async Task<ApiResponseDTO<int>> Handle(CreateFinancialYearCommand request, CancellationToken cancellationToken)
{
    _logger.LogInformation("Starting CreateFinancialYearCommandHandler for request: {@Request}", request);

    // Check if repository is null
    if (_financialYearCommandRepository == null)
    {
        _logger.LogError("FinancialYearCommandRepository is null. Ensure dependency injection is configured.");
        throw new InvalidOperationException("FinancialYearCommandRepository is not initialized.");
    }

    // Map request to entity
    var financialYearEntity = _mapper.Map<Core.Domain.Entities.FinancialYear>(request);
    _logger.LogInformation("Mapped Create FinancialYear Command to FinancialYear entity: {@FinancialYearEntity}", financialYearEntity);

    // Save FinancialYear
    var createdfinancialYear = await _financialYearCommandRepository.CreateAsync(financialYearEntity);

    if (createdfinancialYear == null)
    {
        _logger.LogWarning("Failed to create financial year. Entity is null.");
        return new ApiResponseDTO<int> { IsSuccess = false, Message = "FinancialYear not created" };
    }

    _logger.LogInformation("FinancialYear successfully created with ID: {FYId}", createdfinancialYear.Id);

    // Publish Domain Event
    var domainEvent = new AuditLogsDomainEvent(
        actionDetail: "Create",
        actionCode: createdfinancialYear.Id.ToString(),
        actionName: createdfinancialYear.StartYear,
        details: $"FinancialYear '{createdfinancialYear.StartYear}' was created. FinancialYearID: {createdfinancialYear.Id}",
        module: "FinancialYear"
    );

    await _mediator.Publish(domainEvent, cancellationToken);
    _logger.LogInformation("AuditLogsDomainEvent published for FinancialYear ID: {FYId}", createdfinancialYear.Id);

    // Map result to DTO
    var deptDto = _mapper.Map<FinancialYearDto>(createdfinancialYear);
    if (deptDto == null)
    {
        _logger.LogError("Failed to map created FinancialYear to DTO.");
        return new ApiResponseDTO<int> { IsSuccess = false, Message = "Error mapping FinancialYear" };
    }

    _logger.LogInformation("Returning success response for FinancialYear ID: {FYId}", createdfinancialYear.Id);

    return new ApiResponseDTO<int>
    {
        IsSuccess = true,
        Message = "FinancialYear created successfully",
        Data = createdfinancialYear.Id
    };
}
        // public async Task<ApiResponseDTO<int>> Handle(CreateFinancialYearCommand request, CancellationToken cancellationToken)
        // {

        //     _logger.LogInformation("Starting CreateDepartmentCommandHandler for request: {@Request}", request);

        //                 // Map the request to the entity
        //                 var  financialYearEntity = _mapper.Map<Core.Domain.Entities.FinancialYear>(request);
        //                 _logger.LogInformation("Mapped Create FinancialYear Command to Department entity: {@DepartmentEntity}", financialYearEntity);

        //                 // Save the department
        //                 var createdfinancialYear = await _financialYearCommandRepository.CreateAsync(financialYearEntity);

        //                 if (createdfinancialYear == null)
        //                 {
        //                     _logger.LogWarning("Failed to create department. FinancialYear entity: {@financialYearEntity}", financialYearEntity);
        //                     return new ApiResponseDTO<int> {IsSuccess = false, Message = "FinancialYear not created" };
        //                 }

        //                 _logger.LogInformation("FinancialYear successfully created with ID: {FYId}", createdfinancialYear.Id);

        //                 // Publish the domain event
        //                 var domainEvent = new AuditLogsDomainEvent(
        //                     actionDetail: "Create",
        //                     actionCode: createdfinancialYear.Id.ToString(),
        //                     actionName: createdfinancialYear.StartYear,
        //                     details: $"FinancialYear '{createdfinancialYear.StartYear}' was created. FinancialYearID: {createdfinancialYear.Id}",
        //                     module: "FinancialYear"
        //                 );

        //                 await _mediator.Publish(domainEvent, cancellationToken);
        //                 _logger.LogInformation("AuditLogsDomainEvent published for FinancialYear ID: {FYId}", createdfinancialYear.Id);

        //                 // Map the result to DTO
        //                 var deptDto = _mapper.Map<FinancialYearDto>(createdfinancialYear);
        //                     if (deptDto == null)
        //                     {
        //                         _logger.LogError("Failed to map created FinancialYear to DTO.");
        //                         return new ApiResponseDTO<int> { IsSuccess = false, Message = "Error mapping FinancialYear" };
        //                     }
        //                 _logger.LogInformation("Returning success response for FinancialYear ID: {FYId}", createdfinancialYear.Id);

        //                 return new ApiResponseDTO<int>
        //                 {
        //                     IsSuccess = true,
        //                     Message = "FinancialYear created successfully",
        //                     Data = createdfinancialYear.Id
        //                 };


        // }

        
    }
}