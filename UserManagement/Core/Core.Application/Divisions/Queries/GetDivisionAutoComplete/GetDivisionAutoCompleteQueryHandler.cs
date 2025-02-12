using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using Core.Domain.Entities;
using Core.Application.Divisions.Queries.GetDivisions;
using System.Data;
using Core.Application.Common.Interfaces.IDivision;
using Core.Application.Common.HttpResponse;
using Core.Domain.Events;
using System.Text.Json;
using Core.Application.Users.Queries.GetUsers;



namespace Core.Application.Divisions.Queries.GetDivisionAutoComplete
{
    public class GetDivisionAutoCompleteQueryHandler : IRequestHandler<GetDivisionAutoCompleteQuery,ApiResponseDTO<List<DivisionAutoCompleteDTO>>>
    {
        private readonly IDivisionQueryRepository _divisionRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
         public GetDivisionAutoCompleteQueryHandler(IDivisionQueryRepository divisionRepository, IMapper mapper, IMediator mediator)
         {
            _divisionRepository =divisionRepository;
            _mapper =mapper;
            _mediator = mediator;
         }  
          public async Task<ApiResponseDTO<List<DivisionAutoCompleteDTO>>> Handle(GetDivisionAutoCompleteQuery request, CancellationToken cancellationToken)
          {
                var companies = JsonSerializer.Deserialize<List<UserCompanyDTO>>(request.Companies) ?? new List<UserCompanyDTO>();
             var companylist =   _mapper.Map<List<UserCompany>>(companies);
             
            var result = await _divisionRepository.GetDivision(request.SearchPattern,companylist);
            var division = _mapper.Map<List<DivisionAutoCompleteDTO>>(result);
             //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "",
                    details: $"Division details was fetched.",
                    module:"Division"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<DivisionAutoCompleteDTO>> { IsSuccess = true, Message = "Success", Data = division };            
         } 
    }
}