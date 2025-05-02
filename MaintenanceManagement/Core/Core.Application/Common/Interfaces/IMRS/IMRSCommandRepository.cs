using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MRS.Command.CreateMRS;

namespace Core.Application.Common.Interfaces.IMRS
{
    public interface IMRSCommandRepository
    {
         Task<int> InsertMRSAsync(HeaderRequest headerRequest);
    }
}