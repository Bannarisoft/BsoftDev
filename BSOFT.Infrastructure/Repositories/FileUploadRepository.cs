using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BSOFT.Infrastructure.Repositories
{
    public class FileUploadRepository : IFileUploadService
    {
        public async Task<(bool IsSuccess, string FilePath, string ErrorMessage)> UploadFileAsync(IFormFile file, string uploadPath)
        {
            if (file.Length > 10 * 1024 * 1024)
            {
                throw new ArgumentException("File size exceeds limit");
            }
              string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

        
            string tempPath = Path.Combine(uploadPath, "temp");
            string filePath = Path.Combine(tempPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string finalPath = Path.Combine(uploadPath, fileName);
            File.Move(filePath, finalPath);
            return (true, finalPath, null);;
        }
    }
}