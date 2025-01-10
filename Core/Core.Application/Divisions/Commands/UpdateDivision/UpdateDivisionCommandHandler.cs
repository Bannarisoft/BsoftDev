using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IDivision;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Divisions.Commands.UpdateDivision
{
    public class UpdateDivisionCommandHandler : IRequestHandler<UpdateDivisionCommand, ApiResponseDTO<bool>>
    {
        private readonly IDivisionCommandRepository _divisionRepository;
        private readonly IMapper _imapper;
        public UpdateDivisionCommandHandler(IDivisionCommandRepository divisionRepository,IMapper imapper)
        {
            _divisionRepository =divisionRepository;
            _imapper =imapper;
        }
          public async Task<ApiResponseDTO<bool>> Handle(UpdateDivisionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                 var division  = _imapper.Map<Division>(request);
         
                var divisionresult = await _divisionRepository.UpdateAsync(division);
              
                if(divisionresult)
                {
                    return new ApiResponseDTO<bool>{IsSuccess = true, Message = "Division updated successfully.", Data = true};
                }
                return new ApiResponseDTO<bool>{IsSuccess = false, Message = "Division not updated.", Data = false};
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<bool>{IsSuccess = false, Message = "An error occurred.", ErrorCode = "ERR_UPDATE_DIVISION"};
            }
           
            
        }
        
    }
}