using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ILanguage;
using Core.Application.Language.Queries.GetLanguages;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Language.Commands.CreateLanguage
{
    public class CreateLanguageCommandHandler : IRequestHandler<CreateLanguageCommand, ApiResponseDTO<LanguageDTO>>
    {
        private readonly ILanguageCommand _languageCommand;
        private readonly IMapper _imapper;
        private readonly IMediator _mediator;
        private readonly ILanguageQuery _languageQuery;
        public CreateLanguageCommandHandler(ILanguageCommand languageCommand, IMapper imapper, IMediator mediator, ILanguageQuery languageQuery)
        {
            _languageCommand = languageCommand;
            _imapper = imapper;
            _mediator = mediator;
            _languageQuery = languageQuery;
        }

        public async Task<ApiResponseDTO<LanguageDTO>> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
        {
            var existingLanguage = await _languageQuery.GetByLanguagenameAsync(request.Name);

              if (existingLanguage != null)
              {
                  return new ApiResponseDTO<LanguageDTO>{IsSuccess = false, Message = "Language already exists"};
              }
                var language  = _imapper.Map<Core.Domain.Entities.Language>(request);

                var languageresult = await _languageCommand.CreateAsync(language);

                
                

                var divisionMap = _imapper.Map<LanguageDTO>(languageresult);
                if (languageresult.Id > 0)
                {
                     var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "Create",
                     actionCode: languageresult.Code,
                     actionName: languageresult.Name,
                     details: $"Language '{languageresult.Name}' was created.",
                     module:"Language"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
                 
                    return new ApiResponseDTO<LanguageDTO>{IsSuccess = true, Message = "Language created successfully", Data = divisionMap};
                }
               
                    return new ApiResponseDTO<LanguageDTO>{IsSuccess = false, Message = "Language not created"};
        }
    }
}