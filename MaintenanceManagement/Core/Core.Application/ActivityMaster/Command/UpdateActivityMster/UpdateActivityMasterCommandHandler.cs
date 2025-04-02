using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IActivityMaster;
using FluentValidation;
using MediatR;

namespace Core.Application.ActivityMaster.Command.UpdateActivityMster
{
    public class UpdateActivityMasterCommandHandler  : IRequestHandler<UpdateActivityMasterCommand, ApiResponseDTO<int>>
    {
         private readonly IActivityMasterQueryRepository _activityMasterQueryRepository ;

          private readonly IActivityMasterCommandRepository _activityMasterCommandRepository ;
           private readonly IMapper _mapper;
          private readonly IMediator _mediator;

            private readonly IValidator<UpdateActivityMasterCommand> _validator;


        public UpdateActivityMasterCommandHandler(IActivityMasterQueryRepository activityMasterQueryRepository, IActivityMasterCommandRepository activityMasterCommandRepository, IMapper mapper, IMediator mediator , IValidator<UpdateActivityMasterCommand> validator)
        {
            _activityMasterQueryRepository = activityMasterQueryRepository;
            _activityMasterCommandRepository = activityMasterCommandRepository;
            _mapper = mapper;
            _mediator = mediator;
            _validator = validator;
        }
         public async Task<ApiResponseDTO<int>> Handle(UpdateActivityMasterCommand request, CancellationToken cancellationToken)
        { 
                        // 🔹 Retrieve Existing Record from Query Repository
                 // 🔹 Retrieve Existing Record from Query Repository
          //   var existingRecordDto = await _activityMasterQueryRepository.GetByIdAsync(request.UpdateActivityMaster.ActivityId);
            // if (existingRecordDto == null)
            // {
            //     return new ApiResponseDTO<int>
            //     {
            //         IsSuccess = false,
            //         Message = $"Activity Master with ID {request.UpdateActivityMaster.ActivityId} not found."
            //     };
            // }

            // 🔹 Convert DTO to Domain Entity
            var activityMasterEntity = _mapper.Map<Core.Domain.Entities.ActivityMaster>(request.UpdateActivityMaster);

            // 🔹 Save Changes
        //    var result = await _activityMasterCommandRepository.UpdateAsync(activityMasterEntity);
            var result = await _activityMasterCommandRepository.UpdateAsync(request.UpdateActivityMaster);


            if (result>0)
            {
                return new ApiResponseDTO<int>
                {
                    IsSuccess = true,
                    Message = "Activity Master updated successfully"
                };
            }

            return new ApiResponseDTO<int>
            {
                IsSuccess = false,
                Message = "Activity Master update failed"
            };
        }

        
    }
}