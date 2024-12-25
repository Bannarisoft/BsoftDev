using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BSOFT.Application.Common.Interfaces
{
    public interface IFileUploadService
    {
        Task<(bool IsSuccess, string FilePath, string ErrorMessage)> UploadFileAsync(IFormFile file, string uploadPath);
    }
}