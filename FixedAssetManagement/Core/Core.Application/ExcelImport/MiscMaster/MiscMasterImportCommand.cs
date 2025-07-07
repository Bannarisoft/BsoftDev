using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.ExcelImport.MiscMaster
{
    public class MiscMasterImportCommand : IRequest<ApiResponseDTO<bool>>
    {

      
        public IFormFile File { get; set; }
          public MiscMasterImportCommand(IFormFile file)
        {
            File = file;
        }
           
       

    }
}