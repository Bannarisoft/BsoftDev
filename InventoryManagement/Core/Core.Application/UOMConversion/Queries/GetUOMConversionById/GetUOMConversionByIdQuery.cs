using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using Core.Application.UOMConversion.Queries.GetAllUOMConversion;
using MediatR;

namespace Core.Application.UOMConversion.Queries.GetUOMConversionById
{
    public class GetUOMConversionByIdQuery :IRequest<UOMConversionDto>
    {
        public int Id { get; set; }    
        
    }
}