using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICustomField;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.CustomFields.Commands.CreateCustomField
{
    public class CreateCustomFieldCommandhandler : IRequestHandler<CreateCustomFieldCommand, ApiResponseDTO<int>>
    {
        private readonly ICustomFieldCommand _customFieldCommand;
        private readonly IMapper _imapper;
        private readonly IMediator _mediator;
        public CreateCustomFieldCommandhandler(ICustomFieldCommand customFieldCommand, IMapper imapper, IMediator mediator)
        {
            _customFieldCommand = customFieldCommand;
            _imapper = imapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<int>> Handle(CreateCustomFieldCommand request, CancellationToken cancellationToken)
        {
             var customField  = _imapper.Map<CustomField>(request);

                var customFieldresult = await _customFieldCommand.CreateAsync(customField);
                
                
                if (customFieldresult > 0)
                {
                //     var domainEvent = new AuditLogsDomainEvent(
                //      actionDetail: "Create",
                //      actionCode: "Create custom field",
                //      actionName: "Create",
                //      details: $"Custom field ",
                //      module:"Custom Field"
                //  );
                //  await _mediator.Publish(domainEvent, cancellationToken);
                 
                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = true, 
                        Message = "Custom field created successfully",
                         Data = customFieldresult
                    };
                }
               
                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = false, 
                        Message = "Custom field not created"
                    };
        }
    }
}