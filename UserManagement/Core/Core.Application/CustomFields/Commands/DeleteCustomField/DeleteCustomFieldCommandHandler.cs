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

namespace Core.Application.CustomFields.Commands.DeleteCustomField
{
    public class DeleteCustomFieldCommandHandler : IRequestHandler<DeleteCustomFieldCommand, ApiResponseDTO<bool>>
    {
        private readonly ICustomFieldCommand _customFieldCommand;
        private readonly IMapper _imapper;
        private readonly IMediator _mediator;
        public DeleteCustomFieldCommandHandler(ICustomFieldCommand customFieldCommand, IMapper imapper, IMediator mediator)
        {
            _customFieldCommand = customFieldCommand;
            _imapper = imapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<bool>> Handle(DeleteCustomFieldCommand request, CancellationToken cancellationToken)
        {
            var customField  = _imapper.Map<CustomField>(request);
            var customFieldresult = await _customFieldCommand.DeleteAsync(request.Id, customField);


                  //Domain Event  
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Delete",
                        actionCode: "Delete custom field",
                        actionName: "Delete custom field",
                        details: $"custom field '{request.Id}' was deleted.",
                        module:"custom field"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken);  

                 if(customFieldresult)
                {
                    return new ApiResponseDTO<bool>
                    {
                        IsSuccess = true, 
                        Message = "Custom field deleted successfully."
                    };
                }

                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false, 
                    Message = "Custom field not deleted."
                };
        }
    }
}