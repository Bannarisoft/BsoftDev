using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUOM;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.UOM.Command.UpdateUOM
{
    public class UpdateUOMCommandHandler : IRequestHandler<UpdateUOMCommand, ApiResponseDTO<bool>>
    {
         private readonly IUOMCommandRepository _uomCommandRepository;
        private readonly IUOMQueryRepository _uomQueryRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public UpdateUOMCommandHandler(IUOMCommandRepository uomCommandRepository,IUOMQueryRepository uomQueryRepository,IMapper mapper,IMediator mediator)
        {
           _uomCommandRepository = uomCommandRepository;
           _uomQueryRepository = uomQueryRepository;
           _mapper = mapper;
           _mediator = mediator; 
        }
        public async Task<ApiResponseDTO<bool>> Handle(UpdateUOMCommand request, CancellationToken cancellationToken)
        {
            var existinguom = await _uomQueryRepository.GetByUOMNameAsync(request.UOMName, request.Id);

                if (existinguom != null)
                {
                    return new ApiResponseDTO<bool>{IsSuccess = false, Message = "UOM already exists"};
                }

                     // Check for duplicate GroupName or SortOrder
       var (isNameDuplicate, isSortOrderDuplicate) = await _uomCommandRepository
                                .CheckForDuplicatesAsync(request.UOMName, request.SortOrder, request.Id);

        if (isNameDuplicate || isSortOrderDuplicate)
        {
            string errorMessage = isNameDuplicate && isSortOrderDuplicate
            ? "Both UOMName and Sort Order already exist."
            : isNameDuplicate
            ? "UOM with the same UOMName already exists."
            : "UOM with the same Sort Order already exists.";

            return new ApiResponseDTO<bool>
            {
                IsSuccess = false,
                Message = errorMessage
            };
        }

                 var uom  = _mapper.Map<Core.Domain.Entities.UOM>(request);
         
                var uomresult = await _uomCommandRepository.UpdateAsync(uom);

                
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Update",
                        actionCode: uom.Code,
                        actionName: uom.UOMName,
                        details: $"UOM '{uom.Id}' was updated.",
                        module:"UOM"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken); 
              
                if(uomresult)
                {
                    return new ApiResponseDTO<bool>{IsSuccess = true, Message = "UOM updated successfully."};
                }

                return new ApiResponseDTO<bool>{IsSuccess = false, Message = "UOM not updated."};
        }
    }
}