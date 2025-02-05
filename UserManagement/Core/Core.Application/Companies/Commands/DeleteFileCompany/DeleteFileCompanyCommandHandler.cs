using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ICompany;
using MediatR;

namespace Core.Application.Companies.Commands.DeleteFileCompany
{
    public class DeleteFileCompanyCommandHandler : IRequestHandler<DeleteFileCompanyCommand, ApiResponseDTO<bool>>
    {
        private readonly IFileUploadService _ifileUploadService;
        public DeleteFileCompanyCommandHandler(IFileUploadService ifileUploadService)
        {
            _ifileUploadService = ifileUploadService;
        }

        public async Task<ApiResponseDTO<bool>> Handle(DeleteFileCompanyCommand request, CancellationToken cancellationToken)
        {
            
           var result = await _ifileUploadService.DeleteFileAsync(request.Logo);
           return new ApiResponseDTO<bool>{IsSuccess = true, Message = "File deleted successfully"};
        }
    }
}