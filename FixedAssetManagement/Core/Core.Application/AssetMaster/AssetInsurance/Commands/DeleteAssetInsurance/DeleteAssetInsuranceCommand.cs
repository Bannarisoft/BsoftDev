using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetInsurance.Queries.GetAssetInsurance;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetInsurance.Commands.DeleteAssetInsurance
{
    public class DeleteAssetInsuranceCommand :  IRequest<ApiResponseDTO<GetAssetInsuranceDto>>
    {
        public int Id { get; set; }  
    }
}