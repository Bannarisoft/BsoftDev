using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.WarehouseMaster.Queries.GetParentWarehouseMaster
{
    public class GetParentWarehouseMasterQuery : IRequest<List<GetParentWarehouseDto>>
    {
        
    }
}