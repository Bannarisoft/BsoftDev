using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUOMConversion;
using Core.Application.UOMConversion.Queries.GetAllUOMConversion;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.UOMConversion.Command.UpdateUOMConversion
{
    public class UpdateUOMConversionCommandHandler : IRequestHandler<UpdateUOMConversionCommand, ApiResponseDTO<UOMConversionDto>>
    {
        private readonly IUOMConversionCommandRepository _iUOMConversionCommandRepository;
        private readonly IUOMConversionQueryRepository _uOMConversionQueryRepository;

        private readonly IMapper _mapper;
        private readonly IMediator _mediator;


        public UpdateUOMConversionCommandHandler(IUOMConversionCommandRepository commandRepository, IUOMConversionQueryRepository queryRepository, IMapper mapper, IMediator mediator)
        {
            _iUOMConversionCommandRepository = commandRepository;
            _uOMConversionQueryRepository = queryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

       public async Task<ApiResponseDTO<UOMConversionDto>> Handle(UpdateUOMConversionCommand request, CancellationToken cancellationToken)
        {
        
            var uomEntity = _mapper.Map<Core.Domain.Entities.UOMConversion>(request);
           
           
            var updateResult = await _iUOMConversionCommandRepository.UpdateAsync(request.Id, uomEntity);

        
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: uomEntity.Id.ToString(),
                actionName: "Update UOM Conversion",
                details: $"UOM Conversion '{uomEntity.Id}' was updated successfully.",
                module: "UOM Conversion"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

              var dto = _mapper.Map<UOMConversionDto>(uomEntity);
              return new ApiResponseDTO<UOMConversionDto>
                    {
                        IsSuccess = true,
                        Message = "UOM Conversion updated successfully.",
                        Data = dto
                    };
        }


      
    }
}