using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUser;
using MediatR;

namespace Core.Application.EntityLevelAdmin.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ApiResponseDTO<bool>>
    {
         private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IUserCommandRepository _userRepository;
        private readonly IUserQueryRepository _userQueryRepository;
        public ResetPasswordCommandHandler(IMediator mediator, IMapper mapper, IUserCommandRepository userRepository, IUserQueryRepository userQueryRepository)
        {
            _mediator = mediator;
            _mapper = mapper;
            _userRepository = userRepository;
            _userQueryRepository = userQueryRepository;
        }
        public async Task<ApiResponseDTO<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
              var existingUser = await _userQueryRepository.GetByIdAsync(request.Id);
            if (existingUser == null)
            {
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "User not found."
                    
                };
            }
            _mapper.Map(request, existingUser);

             var RowsUpdated = await _userRepository.UpdateAsync(request.Id, existingUser);
             if(RowsUpdated > 0)
             {
                 return new ApiResponseDTO<bool>
                {
                    IsSuccess = true,
                    Message = "Password updated successfully."
                    
                };
             }
              return new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "Password update failed."
                    
                };
        }
    }
}