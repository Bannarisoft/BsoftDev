using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.WarehouseMaster.GetAllWarehouseMaster;
using MediatR;

namespace Core.Application.WarehouseMaster.GetWarehouseMasterById
{
    public class GetWarehouseMasterByIdQuery : IRequest<ApiResponseDTO<WarehouseMasterDto>>
    {
        public int Id { get; set; }
        
    }
}