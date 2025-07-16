using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUOM;
using Core.Application.UOM.Queries.GetUOMs;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.UOM.Command.DeleteUOM
{
    public class DeleteUOMCommandHandler : IRequestHandler<DeleteUOMCommand, ApiResponseDTO<UOMDto>>
    {
        private readonly IUOMCommandRepository _uomCommandRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public DeleteUOMCommandHandler(IUOMCommandRepository uomCommandRepository,IMediator mediator,IMapper mapper)
        {
            _uomCommandRepository = uomCommandRepository;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<UOMDto>> Handle(DeleteUOMCommand request, CancellationToken cancellationToken)
        {
            var uom  = _mapper.Map<Core.Domain.Entities.UOM>(request);
            var uomresult = await _uomCommandRepository.DeleteAsync(request.Id, uom);


                  //Domain Event  
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Delete",
                        actionCode: uom.Id.ToString(),
                        actionName: uom.Id.ToString(),
                        details: $"UOM '{uom.Id}' was deleted.",
                        module:"UOM"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken);  

                 if(uomresult)
                {
                    return new ApiResponseDTO<UOMDto>{IsSuccess = true, Message = "UOM deleted successfully."};
                }

                return new ApiResponseDTO<UOMDto>{IsSuccess = false, Message = "UOM not deleted."};
        }
    }
}