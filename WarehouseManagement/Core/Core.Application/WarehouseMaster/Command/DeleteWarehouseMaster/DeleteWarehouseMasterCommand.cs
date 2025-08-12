using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.WarehouseMaster.Command.DeleteWarehouseMaster
{
    public class DeleteWarehouseMasterCommand  : IRequest<bool>
    {
        public int  Id { get; set; }
    }
}