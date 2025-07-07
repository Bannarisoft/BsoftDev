using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Item.ItemMaster.Queries
{
    public class GetItemMasterQuery : IRequest<List<GetItemMasterDto>>
    {
        public string? OldUnitId { get; set; } 
        public string? Grpcode { get; set; } 
        public string? ItemCode { get; set; } 
        public string? ItemName { get; set; } 


    }
}