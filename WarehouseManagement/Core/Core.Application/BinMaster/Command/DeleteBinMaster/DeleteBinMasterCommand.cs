using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.BinMaster.Command.DeleteBinMaster
{
    public class DeleteBinMasterCommand : IRequest<bool>
    {
        public int  Id { get; set; }
        
    }
}