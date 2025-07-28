using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Workflow.Common;
using BackgroundService.Application.Workflow.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BackgroundService.Infrastructure.Repositories.Workflow
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
         private readonly IHttpContextAccessor _httpContext;
         public FileStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContext)
         {
             _env = env;
            _httpContext = httpContext;
         }

        public async Task<FileUploadResult> SaveFileAsync(IFormFile file, string subDirectory)
        {
             var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
              var folderPath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), subDirectory);
              Directory.CreateDirectory(folderPath);
        
              var relativePath = Path.Combine(subDirectory, fileName);
              var fullPath = Path.Combine(folderPath, fileName);
        
              using var stream = new FileStream(fullPath, FileMode.Create);
              await file.CopyToAsync(stream);
        
              var request = _httpContext.HttpContext?.Request;
              var fileUrl = request is not null
                  ? $"{request.Scheme}://{request.Host}/{relativePath.Replace("\\", "/")}"
                  : relativePath;
        
              return new FileUploadResult
              {
                  FileName = fileName,
                  RelativePath = relativePath.Replace("\\", "/"),
                  Url = fileUrl
              };
        }
    }
}