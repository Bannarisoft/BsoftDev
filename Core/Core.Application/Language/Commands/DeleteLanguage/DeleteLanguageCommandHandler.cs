using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ILanguage;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Language.Commands.DeleteLanguage
{
    public class DeleteLanguageCommandHandler : IRequestHandler<DeleteLanguageCommand, ApiResponseDTO<bool>>
    {
        private readonly ILanguageCommand _languageCommand;
        private readonly IMediator _mediator;
        private readonly ILanguageQuery _languageQuery;
        private readonly IMapper _mapper;

        public DeleteLanguageCommandHandler(ILanguageCommand languageRepository, IMediator mediator, ILanguageQuery languageQuery, IMapper mapper)
        {
            _languageCommand = languageRepository;
            _mediator = mediator;
            _languageQuery = languageQuery;
            _mapper = mapper;
        }

        public async Task<ApiResponseDTO<bool>> Handle(DeleteLanguageCommand request, CancellationToken cancellationToken)
        {
            
            var language  = _mapper.Map<Core.Domain.Entities.Language>(request);
         
                var languageresult = await _languageCommand.DeleteAsync(language.Id, language);

                
                   
              
                if(languageresult)
                {
                     var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Delete",
                        actionCode: "",
                        actionName:"",
                        details: $"Language '{language.Id}' was deleted.",
                        module:"Language"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken); 

                    return new ApiResponseDTO<bool>{IsSuccess = true, Message = "Language deleted successfully."};
                }

                return new ApiResponseDTO<bool>{IsSuccess = false, Message = "Language not deleted."};
        }
    }
}