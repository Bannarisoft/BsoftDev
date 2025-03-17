using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.DepreciationDetail.Queries.GetDepreciationDetail;
using MediatR;

namespace Core.Application.DepreciationDetail.Commands.UpdateDepreciationDetail
{
    public class UpdateDepreciationDetailCommand  :  IRequest<ApiResponseDTO<DepreciationDto>>  
    {
        public int companyId { get; set; } 
        public int unitId { get; set; } 
        public int finYearId { get; set; }        
        public int depreciationType { get; set; }      
        public int depreciationPeriod { get; set; }  
    }
}