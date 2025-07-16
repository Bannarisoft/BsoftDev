using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.EntityLevelAdmin.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<ApiResponseDTO<bool>>
    {
        public int UserId { get; set; }
        public string? VerificationCode { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}