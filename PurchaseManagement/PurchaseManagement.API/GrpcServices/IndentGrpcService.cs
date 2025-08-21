using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IPurchaseIndent;
using Core.Domain.Entities;
using Grpc.Core;
using GrpcServices.BackgroundService;

namespace PurchaseManagement.API.GrpcServices
{
    public class IndentGrpcService : IndentByIdService.IndentByIdServiceBase
    {
        private readonly IPurchaseIndentGrpcQuery _purchaseIndentGrpcQuery;
        public IndentGrpcService(IPurchaseIndentGrpcQuery purchaseIndentGrpcQuery)
        {
            _purchaseIndentGrpcQuery = purchaseIndentGrpcQuery;
        }
         public override async Task<IndentResponse> GetIndentById(IndentRequest request, ServerCallContext context)
           {
               var indents = await _purchaseIndentGrpcQuery.GetByIdGrpcAsync(request.Id);

               var response = new IndentResponse();

                var dto = new IndentDto
               {
                   Id = indents.Id,
                   IndentNumber = indents.IndentNumber,
                   IndentDate = indents.IndentDate.ToString("yyyy-MM-dd"),
                   IndentTypeId = indents.IndentTypeId,
                   UnitId = indents.UnitId,
                   Purpose = indents.Purpose ?? ""
               };

              foreach (var d in indents.IndentDetails ?? Enumerable.Empty<IndentDetail>())
            {
                dto.IndentDetails.Add(new IndentDetailDto
                {
                    Id = d.Id,
                    IndentHeaderId = d.IndentHeaderId,
                    ItemId = d.ItemId,
                    QuantityRequired = Convert.ToDouble(d.QuantityRequired),
                    RequiredDate = d.RequiredDate.ToString("yyyy-MM-dd"),
                    TotalEstimatedCost = Convert.ToDouble(d.TotalEstimatedCost),
                    PRConsumptionDays = d.PRConsumptionDays,
                    Remark = d.Remark ?? ""
                });
            }

            foreach (var m in indents.IndentDepartmentMappings ?? Enumerable.Empty<IndentDepartmentMapping>())
              {
                  dto.IndentDepartmentMappings.Add(new IndentDepartmentMappingDto
                  {
                      Id = m.Id,
                      IndentHeaderId = m.IndentHeaderId,
                      DepartmentId = m.DepartmentId
                  });
              }

               return response;
           }
    }
}